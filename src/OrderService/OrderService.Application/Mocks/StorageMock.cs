using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Mocks;

public interface IStorageServiceMock
{
    Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items);
    Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> products);
    Task<Result> ReduceCountOfProducts(IEnumerable<DecreaseQuantity> items);
    Task<IEnumerable<ProductStorage>> GetProductStorage(IEnumerable<ProductQuantity> products);
    Task<Result> ReturnProductsToStorage(IEnumerable<ProductQuantity> products);
}

public class StorageServiceMock : IStorageServiceMock
{
    public Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items) 
        => Task.FromResult(new List<StockCheckResult>().AsEnumerable());
    public Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> productId) 
        => Task.FromResult(DateTime.Now);
    public async Task<Result> ReduceCountOfProducts(IEnumerable<DecreaseQuantity> items) => Result.Ok();
    public Task<IEnumerable<ProductStorage>> GetProductStorage(IEnumerable<ProductQuantity> products) 
        => Task.FromResult(new List<ProductStorage>().AsEnumerable());

    public async Task<Result> ReturnProductsToStorage(IEnumerable<ProductQuantity> products) => Result.Ok();
}