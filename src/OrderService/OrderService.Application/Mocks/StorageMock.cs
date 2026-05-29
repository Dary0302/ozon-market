using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Mocks;

public interface IStorageServiceMock
{
    Task<List<StockCheckResult>> CheckStock(List<ProductQuantity> items);
    Task<DateTime> GetDeliveryDate(Guid pvzId, Guid[] productId);
    Task ReduceCountOfProducts(List<ProductQuantity> items);
}

public class StorageServiceMock : IStorageServiceMock
{
    public Task<List<StockCheckResult>> CheckStock(List<ProductQuantity> items) => Task.FromResult(new List<StockCheckResult>());
    public Task<DateTime> GetDeliveryDate(Guid pvzId, Guid[] productId) => Task.FromResult(DateTime.Now);
    public Task ReduceCountOfProducts(List<ProductQuantity> items) => Task.CompletedTask;
}