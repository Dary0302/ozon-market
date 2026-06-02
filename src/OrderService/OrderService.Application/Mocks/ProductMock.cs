using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Mocks;

public interface IProductServiceMock
{
    Task<decimal> CalculateAmount(IEnumerable<ProductQuantity> items);
}

public class ProductServiceMock : IProductServiceMock
{
    public Task<decimal> CalculateAmount(IEnumerable<ProductQuantity> items) => Task.FromResult<decimal>(1000);
}