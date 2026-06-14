using Core.Common.Kafka.Contracts.Models;

namespace Core.Common.Kafka.Contracts.Services;

public interface IProductService
{
    Task<IEnumerable<ProductPrice>> GetPrices(IEnumerable<ProductPriceRequest> requests, CancellationToken ct);
    Task<IEnumerable<ProductPrice>> GetAmount(IEnumerable<ProductQuantity> productQuantities, CancellationToken ct);
}