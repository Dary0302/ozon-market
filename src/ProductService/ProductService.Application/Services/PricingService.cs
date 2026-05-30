using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class PricingService(IPriceRepository priceRepository) : IPricingService
{
    public async Task<Result<decimal>> CalculateAmount(List<ProductQuantity> products)
    {
        var productsIds = products
            .Select(product => product.ProductId)
            .ToList();

        var prices = await priceRepository.GetPrices(productsIds);

        if (prices.Count != productsIds.Count)
        {
            return Result.Fail("Цена на один или несколько товаров не найдена");
        }

        var sum = prices.Sum(price => (decimal)price.Cost * (1 - price.Discount / 100));

        return Result.Ok(sum);
    }

    public Task<Result<decimal>> GetActualPrice(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SetDiscount(Guid productId, decimal discountPercent)
    {
        throw new NotImplementedException();
    }
}