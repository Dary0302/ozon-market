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
    IStorageServiceMock storageServiceMock,
    IProductServiceMock productServiceMock) : IOrderManagementService
{
    public async Task<Result<Guid>> Create(Guid pvzId, decimal clientAmount, IEnumerable<ProductQuantity> products)
    {
        //TODO: перевести на реальное общение между сервисами
        var normalizedProducts = NormalizeProducts(products);

        var stockTask = storageServiceMock.CheckStock(normalizedProducts);
        var deliveryTask = storageServiceMock.GetDeliveryDate(pvzId, normalizedProducts);
        var amountTask = productServiceMock.CalculateAmount(normalizedProducts);
        await Task.WhenAll(stockTask, deliveryTask, amountTask);

        var calculatedAmount = amountTask.Result;
        if (!IsAmountValid(calculatedAmount, clientAmount))
            return Result.Fail(OrderErrors.InvalidAmount());

        var lackingProducts = GetLackingProducts(stockTask.Result);
        if (lackingProducts.Any())
            return Result.Fail(OrderErrors.InsufficientStock(lackingProducts));

        var order = new Order(calculatedAmount, pvzId, deliveryTask.Result);
        var items = normalizedProducts
            .Select(product => new OrderItem(order.Id, product.ProductId, product.Quantity)).ToList();
        
        //TODO: подумать над механизмом единых транзакций для бд и сервисов, вероятно, нужен паттерн Saga
        await orderRepository.Create(order);
        await orderItemRepository.Add(items);
        await storageServiceMock.ReduceCountOfProducts(normalizedProducts);
        
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

    public async Task<Result<Order?>> GetById(Guid id)
    {
        var result = await orderRepository.GetById(id);
        return Result.Ok(result);
    }

    public async Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize)
    {
        var result = await orderRepository.GetAll(pageNumber, pageSize);
        return Result.Ok(result);
    }

    public async Task<Result<Guid>> UpdateStatus(Guid id, Status newStatus)
    {
        var order = await orderRepository.GetById(id);

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

        await orderRepository.Save(order);

        return Result.Ok(order.Id);
    }

    public async Task<Result> Delete(Guid id)
    {
        await orderRepository.Delete(id);
        return Result.Ok();
    }

    public async Task<Result<OrderInfo>> GetInfoById(Guid id)
    {
        var orderTask = orderRepository.GetById(id);
        var itemsTask = orderItemRepository.GetAllByOrderId(id);
        
        await Task.WhenAll(orderTask, itemsTask);

        var order = orderTask.Result;
        if (order == null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        return Result.Ok(new OrderInfo(order, itemsTask.Result));
    }

    public async Task<Result<PagedResult<OrderInfo>>> GetAllInfo(int pageNumber, int pageSize)
    {
        var result = await orderInfoRepository.GetAll(pageNumber, pageSize);
        return Result.Ok(result);
    }
}