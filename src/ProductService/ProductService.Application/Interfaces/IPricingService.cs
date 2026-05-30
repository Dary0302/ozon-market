using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IPricingService
{
    Task<Result<decimal>> CalculateAmount(List<ProductQuantity> products);

    Task<Result<decimal>> GetActualPrice(Guid productId);

    Task<Result<bool>> SetDiscount(Guid productId, decimal discountPercent);
}