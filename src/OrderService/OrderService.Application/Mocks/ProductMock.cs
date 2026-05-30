using OrderService.Domain;

namespace OrderService.Application.Mocks;

public interface IProductServiceMock
{
    Task<decimal> CalculateAmount(List<ProductQuantity> items);
}

public class ProductServiceMock : IProductServiceMock
{
    public Task<decimal> CalculateAmount(List<ProductQuantity> items) => Task.FromResult<decimal>(1000);
}