using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Mocks;

public interface IStorageServiceMock
{
    Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items);
    Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> products);
    Task ReduceCountOfProducts(IEnumerable<ProductQuantity> items);
}

public class StorageServiceMock : IStorageServiceMock
{
    public Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items) 
        => Task.FromResult(new List<StockCheckResult>().AsEnumerable());
    public Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> productId) => Task.FromResult(DateTime.Now);
    public Task ReduceCountOfProducts(IEnumerable<ProductQuantity> items) => Task.CompletedTask;
}