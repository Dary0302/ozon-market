using Core.Common.Errors;
using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class PricingService(IPriceRepository priceRepository) : IPricingService
{
    public async Task<Result<decimal>> CalculateAmount(IEnumerable<ProductQuantity> products)
    {
        var productsIds = products
            .Select(product => product.ProductId)
            .ToList();

        var prices = (await priceRepository.GetPrices(productsIds)).ToList();

        if (prices.Count != productsIds.Count)
        {
            return Result.Fail(AppError.NotFound("Цена на один или несколько товаров не найдена"));
        }

        var sum = prices.Sum(GetCostWithDiscount!);

        return Result.Ok(sum);
    }

    public async Task<Result<decimal>> GetActualPrice(Guid productId)
    {
        var price = await priceRepository.GetPrice(productId);

        if (price is null)
        {
            return Result.Fail(AppError.NotFound("Цена на товар не найдена"));
        }

        var actualPrice = GetCostWithDiscount(price);

        return Result.Ok(actualPrice);
    }

    public async Task<Result> SetPrice(Price newPrice)
    {
        if (newPrice.Cost < 0)
        {
            return Result.Fail(AppError.Validation("Цена не может быть меньше 0"));
        }
        
        if (newPrice.Discount is <= 0 or > 100)
        {
            return Result.Fail(AppError.Validation("Скидка не может быть меньше 0%, либо больше 100%"));
        }
        
        await priceRepository.SetPrice(newPrice);

        return Result.Ok();
    }
    
    private static decimal GetCostWithDiscount(Price price)
    {
        return (decimal)price.Cost * (1 - price.Discount / 100);
    }
}