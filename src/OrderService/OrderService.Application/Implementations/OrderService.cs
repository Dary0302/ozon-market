using Core.Common.DbHelpers.Interfaces;
using FluentResults;
using OrderService.Application.Interfaces;
using OrderService.Application.Mocks;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Domain.Exseptions;

namespace OrderService.Application.Implementations;

public class OrderService(IOrderRepository orderRepository, 
    IOrderItemRepository orderItemRepository, 
    IOrderInfoRepository orderInfoRepository,
    IStorageServiceMock storageServiceMock,
    IProductServiceMock productServiceMock) : IOrderService
{
    public async Task<Result<Guid>> Create(Guid pvzId, decimal clientAmount, List<ProductQuantity> products)
    {
        //TODO: перевести на реальное общение между сервисами
        var normalizedProducts = NormalizeProducts(products);
        var productIds = normalizedProducts.Select(x => x.ProductId).Distinct().ToArray();

        var stockTask = storageServiceMock.CheckStock(normalizedProducts);
        var deliveryTask = storageServiceMock.GetDeliveryDate(pvzId, productIds);
        var amountTask = productServiceMock.CalculateAmount(normalizedProducts);
        await Task.WhenAll(stockTask, deliveryTask, amountTask);

        var calculatedAmount = amountTask.Result;
        if (!IsAmountValid(calculatedAmount, clientAmount))
            return Result.Fail(OrderErrors.InvalidAmount());

        var lackingProducts =GetLackingProducts( stockTask.Result);
        if (lackingProducts.Any())
            return Result.Fail(OrderErrors.InsufficientStock(lackingProducts));

        var order = new Order(calculatedAmount, pvzId, deliveryTask.Result);
        var items = normalizedProducts
            .Select(p => new OrderItem(order.Id, p.ProductId, p.Quantity))
            .ToList();
        
        //TODO: подумать над механизмом единых транзакций для бд и сервисов, вероятно, нужен паттерн Saga
        await orderRepository.Create(order);
        await orderItemRepository.Add(items);
        await storageServiceMock.ReduceCountOfProducts(normalizedProducts);
        
        return order.Id;
    }
    
    private List<ProductQuantity> NormalizeProducts(
        List<ProductQuantity> products)
    {
        return products
            .GroupBy(x => x.ProductId)
            .Select(x => new ProductQuantity(
                x.Key,
                x.Sum(y => y.Quantity)))
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
            .Where(p => p.Difference < 0)
            .Select(p => new LackingProduct(
                p.ProductId,
                Math.Abs(p.Difference)))
            .ToList();
    }

    public async Task<Result<Order?>> GetById(Guid id) => await orderRepository.GetById(id);

    public async Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize) 
        => await orderRepository.GetAll(pageNumber, pageSize);

    public async Task<Result<Guid>> UpdateStatus(Guid id, Status status)
    {
        //TODO: подумать, где валидировать смену статуса
        var result = await orderRepository.UpdateStatus(id, status);
        return result;
    }

    public async Task Delete(Guid id) => await orderRepository.Delete(id);

    public async Task<Result<OrderInfo>> GetInfoById(Guid id)
    {
        var orderTask = orderRepository.GetById(id);
        var itemsTask = orderItemRepository.GetAllByOrderId(id);
        
        await Task.WhenAll(orderTask, itemsTask);

        var order = orderTask.Result;
        if (order == null)
            return Result.Fail(OrderErrors.NotFound(id));
        
        return new OrderInfo(order, itemsTask.Result);
    }

    public async Task<Result<PagedResult<OrderInfo>>> GetAllInfo(int pageNumber, int pageSize) 
        => await  orderInfoRepository.GetAll(pageNumber, pageSize);
}