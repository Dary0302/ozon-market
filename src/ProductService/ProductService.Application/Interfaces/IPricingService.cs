using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IPricingService
{
    Task<Result<decimal>> CalculateAmount(IEnumerable<ProductQuantity> products);

    Task<Result<decimal>> GetActualPrice(Guid productId);

    Task<Result> SetPrice(Price newPrice);
}