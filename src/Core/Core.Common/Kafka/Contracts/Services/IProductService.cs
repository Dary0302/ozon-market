using Core.Common.Kafka.Contracts.Models;

namespace Core.Common.Kafka.Contracts.Services;

public interface IProductService
{
    Task<IEnumerable<ProductPrice>> GetPrices(ProductPriceRequest request, CancellationToken ct);
    Task<decimal> GetAmount(IEnumerable<ProductQuantity> productQuantities, CancellationToken ct);
}