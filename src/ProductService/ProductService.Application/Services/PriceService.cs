using Core.Common.Errors;
using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class PriceService(IPriceRepository priceRepository) : IPriceService
{
    public async Task<Result<decimal>> CalculateAmount(IEnumerable<ProductQuantity> products, CancellationToken cancellationToken)
    {
        var productQuantities = products.ToList();

        var productIds = productQuantities
            .Select(x => x.ProductId)
            .ToList();

        var prices = (await priceRepository.GetPrices(productIds, cancellationToken)).ToList();

        if (prices.Count != productIds.Count)
        {
            return Result.Fail(AppError.NotFound("Цена на один или несколько товаров не найдена"));
        }

        var pricesByProductId = prices.ToDictionary(price => price!.ProductId);

        var sum = productQuantities.Sum(product =>
        {
            var price = pricesByProductId[product.ProductId];

            return GetCostWithDiscount(price!) * product.Quantity;
        });

        return Result.Ok(sum);
    }

    public async Task<Result<decimal>> GetActualPrice(Guid productId, DateTime? priceDate, CancellationToken cancellationToken)
    {
        var price = await priceRepository.GetPrice(productId, priceDate, cancellationToken);

        if (price is null)
        {
            return Result.Fail(AppError.NotFound("Цена на товар не найдена"));
        }

        var actualPrice = GetCostWithDiscount(price);

        return Result.Ok(actualPrice);
    }

    public async Task<Result> SetPrice(Price newPrice, CancellationToken cancellationToken)
    {
        if (newPrice.Cost < 0)
        {
            return Result.Fail(AppError.Validation("Цена не может быть меньше 0"));
        }
        
        if (newPrice.Discount is <= 0 or > 100)
        {
            return Result.Fail(AppError.Validation("Скидка не может быть меньше 0%, либо больше 100%"));
        }
        
        await priceRepository.SetPrice(newPrice, cancellationToken);

        return Result.Ok();
    }
    
    private static decimal GetCostWithDiscount(Price price)
    {
        return (decimal)price.Cost * (1 - price.Discount / 100);
    }
}