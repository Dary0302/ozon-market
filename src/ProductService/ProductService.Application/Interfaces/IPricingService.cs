using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IPricingService
{
    Task<Result<decimal>> CalculateAmount(IEnumerable<ProductQuantity> products, CancellationToken cancellationToken);

    Task<Result<decimal>> GetActualPrice(Guid productId, DateTime? priceDate, CancellationToken cancellationToken);

    Task<Result> SetPrice(Price newPrice, CancellationToken cancellationToken);
}