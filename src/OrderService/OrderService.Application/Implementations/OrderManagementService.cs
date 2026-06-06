using Core.Common.DbHelpers.Interfaces;
using FluentResults;
using OrderService.Application.Interfaces;
using OrderService.Application.Mocks;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Domain.Exseptions;

namespace OrderService.Application.Implementations;

public class OrderManagementService(IOrderRepository orderRepository, 
    IOrderItemRepository orderItemRepository, 
    IOrderInfoRepository orderInfoRepository,
    IUnitOfWork unitOfWork,
    IStorageServiceMock storageServiceMock,
    IProductServiceMock productServiceMock) : IOrderManagementService
{
    public async Task<Result<Guid>> Create(Guid pvzId, decimal clientAmount, 
        IEnumerable<ProductQuantity> products, CancellationToken cancellationToken)
    {
        //TODO: перевести на реальное общение между сервисами
        var normalizedProducts = NormalizeProducts(products);

        var stockTask = storageServiceMock.CheckStock(normalizedProducts);
        var deliveryDateTask = storageServiceMock.GetDeliveryDate(pvzId, normalizedProducts);
        var storageTask = storageServiceMock.GetProductStorage(normalizedProducts);
        var amountTask = productServiceMock.CalculateAmount(normalizedProducts);
        await Task.WhenAll(stockTask, deliveryDateTask, amountTask, storageTask);

        var calculatedAmount = amountTask.Result;
        if (!IsAmountValid(calculatedAmount, clientAmount))
            return Result.Fail(OrderErrors.InvalidAmount());

        var lackingProducts = GetLackingProducts(stockTask.Result);
        if (lackingProducts.Any())
            return Result.Fail(OrderErrors.InsufficientStock(lackingProducts));

        var order = new Order(calculatedAmount, pvzId, deliveryDateTask.Result);
        var items = normalizedProducts
            .Select(product => new OrderItem(order.Id, product.ProductId, product.Quantity)).ToList();
        var productStock = PrepareProductStock(storageTask.Result, normalizedProducts);

        await unitOfWork.ExecuteInTransaction(async () =>
        {
            await orderRepository.Create(
                order,
                unitOfWork.CurrentConnection,
                unitOfWork.CurrentTransaction,
                cancellationToken);

            await orderItemRepository.Add(
                items,
                unitOfWork.CurrentConnection,
                unitOfWork.CurrentTransaction,
                cancellationToken);
        });
        
        //TODO: внести в кафку
        await storageServiceMock.ReduceCountOfProducts(productStock);
        
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

    private IEnumerable<DecreaseQuantity> PrepareProductStock(IEnumerable<ProductStorage> productsStorage, 
        IEnumerable<ProductQuantity> productsStock)
    {
        return productsStock
            .Join(
                productsStorage,
                product => product.ProductId,
                storage => storage.ProductId,
                (product, storage) => new DecreaseQuantity(
                    product.ProductId,
                    storage.StorageId,
                    product.Quantity));
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
            Status.Canceled => order.Cancel(),
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

    public async Task<Result<OrderInfo>> GetInfoById(Guid id, CancellationToken cancellationToken)
    {
        var orderTask = orderRepository.GetById(id, cancellationToken);
        var itemsTask = orderItemRepository.GetAllByOrderId(id, cancellationToken);
        
        await Task.WhenAll(orderTask, itemsTask);

        var order = orderTask.Result;
        
        if (order == null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        return Result.Ok(new OrderInfo(order, itemsTask.Result));
    }

    public async Task<Result<PagedResult<OrderInfo>>> GetAllInfo(int pageNumber, int pageSize, 
        CancellationToken cancellationToken)
    {
        var result = await orderInfoRepository.GetAll(pageNumber, pageSize, cancellationToken);
        return Result.Ok(result);
    }
}