using Core.Common.DbHelpers.Interfaces;
using Core.Common.Errors;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Contracts.Services;
using FluentResults;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Domain.Exseptions;

namespace OrderService.Application.Implementations;

public class OrderManagementService(IOrderRepository orderRepository, 
    IOrderItemRepository orderItemRepository, 
    IOrderInfoRepository orderInfoRepository,
    IUnitOfWork unitOfWork,
    IStorageService storageService,
    IProductService productService) : IOrderManagementService
{
    public async Task<Result<Guid>> Create(Guid pvzId, decimal clientAmount, 
        IEnumerable<ProductQuantity> products, CancellationToken cancellationToken)
    {
        var normalizedProducts = NormalizeProducts(products);
        
        var stockTask = storageService.CheckStock(normalizedProducts, cancellationToken);
        var deliveryDateTask = storageService.GetDeliveryDate(pvzId, normalizedProducts, cancellationToken);
        var storageRecordsTask = storageService.GetOrderStorageRecords(pvzId, normalizedProducts, cancellationToken);
        var amountTask = productService.GetAmount(normalizedProducts, cancellationToken);

        await Task.WhenAll(stockTask, deliveryDateTask, amountTask, storageRecordsTask);

        var amount = await amountTask;
        var stock = await stockTask;
        var deliveryDate = await deliveryDateTask;
        var storageRecords = await storageRecordsTask;

        if (!IsAmountValid(amount, clientAmount))
            return Result.Fail(OrderErrors.InvalidAmount());

        var lackingProducts = GetLackingProducts(stock);
        if (lackingProducts.Any())
            return Result.Fail(OrderErrors.InsufficientStock(lackingProducts));

        var order = new Order(amount, pvzId, deliveryDate);
        var items = normalizedProducts
            .Select(product => new OrderItem(order.Id, product.ProductId, product.Quantity)).ToList();

        await unitOfWork.ExecuteInTransaction(async () =>
        {
            await orderRepository.Create(order, unitOfWork.CurrentConnection, 
                unitOfWork.CurrentTransaction, cancellationToken);
            await orderItemRepository.Add(items, unitOfWork.CurrentConnection, 
                unitOfWork.CurrentTransaction, cancellationToken);
        });

        await storageService.ReduceCountOfProducts(storageRecords, cancellationToken);

        return Result.Ok(order.Id);
    }
    
    private List<ProductQuantity> NormalizeProducts(
        IEnumerable<ProductQuantity> products)
    {
        return products
            .GroupBy(product => product.ProductId)
            .Select(product => new ProductQuantity(
                product.Key,
                product.Sum(item => item.Quantity)))
            .ToList();
    }
    
    private bool IsAmountValid(
        decimal calculatedAmount,
        decimal clientAmount)
    {
        return decimal.Round(calculatedAmount, 2)
               == decimal.Round(clientAmount, 2);
    }
    
    private List<LackingProduct> GetLackingProducts(
        IEnumerable<StockCheckResult> stock)
    {
        return stock
            .Where(stockCheck => stockCheck.Difference < 0)
            .Select(stockCheck => new LackingProduct(
                stockCheck.ProductId,
                Math.Abs(stockCheck.Difference)))
            .ToList();
    }

    public async Task<Result<Order>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetById(id, cancellationToken);

        if (result == null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        return Result.Ok(result);
    }

    public async Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize, 
        CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetAll(pageNumber, pageSize, cancellationToken);
        return Result.Ok(result);
    }

    public async Task<Result<Guid>> UpdateStatus(Guid id, Status newStatus, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetById(id, cancellationToken);

        if (order is null)
            return Result.Fail(OrderErrors.NotFound(id));

        Result result = newStatus switch
        {
            Status.Paid => order.Pay(),
            Status.InAssembly => order.Collect(),
            Status.TransferredForDelivery => order.TransferForDelivery(),
            Status.Delivered => order.Complete(),
            _ => Result.Fail(OrderErrors.InvalidStatusTransition())
        };

        if (result.IsFailed)
            return Result.Fail(result.Errors);


        await unitOfWork.ExecuteInTransaction(async () =>
            await orderRepository.Save(order, unitOfWork.CurrentConnection, 
                unitOfWork.CurrentTransaction, cancellationToken));

        return Result.Ok(order.Id);
    }

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken)
    {
        await unitOfWork.ExecuteInTransaction(async () =>
            await orderRepository.Delete(id, unitOfWork.CurrentConnection, 
                unitOfWork.CurrentTransaction, cancellationToken));
        return Result.Ok();
    }

    public async Task<Result<OrderInfoWithPrice>> GetInfoById(Guid id, CancellationToken cancellationToken)
    {
        var order  = await orderRepository.GetById(id, cancellationToken);
        if (order == null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        var items = (await orderItemRepository.GetAllByOrderId(id, cancellationToken)).ToList();
        var productIds = items.Select(item => item.ProductId);
        
        var request = new ProductPriceRequest(order.CreatedOn, productIds);
        var prices = await productService.GetPrices(request, cancellationToken);
        
        var priceMap = prices.ToDictionary(
            product => product.ProductId, product => product.Cost);
        if (!items.All(item => priceMap.ContainsKey(item.ProductId)))
            return Result.Fail(AppError.NotFound("Цена на товар не найдена"));
        
        var itemsWithPrice = items.Select(item => new OrderItemWithPrice(
            item.ProductId,
            item.Quantity,
            priceMap[item.ProductId]));
        
        return Result.Ok(new OrderInfoWithPrice(order, itemsWithPrice));
    }

    public async Task<Result<PagedResult<OrderInfoWithPrice>>> GetAllInfo(int pageNumber, int pageSize, 
        CancellationToken cancellationToken)
    {
        var orderInfos = await orderInfoRepository.GetAll(pageNumber, pageSize, cancellationToken);

        if (!orderInfos.Items.Any())
            return Result.Ok(new PagedResult<OrderInfoWithPrice>(
                Enumerable.Empty<OrderInfoWithPrice>(), orderInfos.TotalCount));

        var result = new List<OrderInfoWithPrice>();

        foreach (var orderInfo in orderInfos.Items)
        {
            var request = new ProductPriceRequest(
                orderInfo.Order.CreatedOn,
                orderInfo.OrderItems.Select(i => i.ProductId).Distinct());
            var prices = await productService.GetPrices(request, cancellationToken);
            var priceMap = prices.ToDictionary(price => price.ProductId, price => price.Cost);
            var missing = orderInfo.OrderItems
                .Select(i => i.ProductId)
                .Where(id => !priceMap.ContainsKey(id))
                .ToList();
            if (!orderInfo.OrderItems.All(item => priceMap.ContainsKey(item.ProductId)))
                return Result.Fail(AppError.NotFound(
                    $"Цена не найдена для товаров: {string.Join(", ", missing)}"));

            result.Add(new OrderInfoWithPrice(
                orderInfo.Order,
                orderInfo.OrderItems.Select(item => new OrderItemWithPrice(
                    item.ProductId,
                    item.Quantity,
                    priceMap[item.ProductId]))));
        }

        return Result.Ok(new PagedResult<OrderInfoWithPrice>(result, orderInfos.TotalCount));
    }

    public async Task<Result<Guid>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetById(id, cancellationToken);
        if (order is null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        var cancelResult = order.Cancel();
        if (cancelResult.IsFailed)
            return Result.Fail(cancelResult.Errors);
        
        await unitOfWork.ExecuteInTransaction(async () =>
            await orderRepository.Save(order, unitOfWork.CurrentConnection, 
                unitOfWork.CurrentTransaction, cancellationToken));
        var items = await orderItemRepository.GetAllByOrderId(id, cancellationToken);
        var productQuantities = items
            .Select(item => new ProductQuantity(item.ProductId, item.Quantity));
        
        await storageService.ReturnProductsToStorage(productQuantities, cancellationToken);

        return Result.Ok(id);
    }
}