using Core.Common.Errors;
using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using ProductService.Application.Interfaces;
using ProductService.Domain;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Services;

public class PriceService(IPriceRepository priceRepository) : IPriceService
{
    public async Task<Result<decimal>> CalculateAmount(
        IEnumerable<ProductQuantity> products,
        CancellationToken cancellationToken)
    {
        var productQuantities = products.ToList();

        var productIds = productQuantities
            .Select(productQuantity => productQuantity.ProductId)
            .ToList();

        var prices = (await priceRepository.GetPrices(productIds, null, cancellationToken)).ToList();

        if (prices.Count != productIds.Count)
        {
            return Result.Fail(AppError.NotFound("Цена на один или несколько товаров не найдена"));
        }

        var pricesByProductId = prices.ToDictionary(price => price.ProductId);

        var sum = productQuantities.Sum(product =>
        {
            var price = pricesByProductId[product.ProductId];

            return GetCostWithDiscount(price) * product.Quantity;
        });

        return Result.Ok(sum);
    }

    public async Task<Result<ProductPrice>> GetActualPrice(
        Guid productId,
        DateTime? priceDate,
        CancellationToken cancellationToken)
    {
        var price = await priceRepository.GetPrice(productId, priceDate, cancellationToken);

        if (price is null)
        {
            return Result.Fail(AppError.NotFound("Цена на товар не найдена"));
        }

        var productPrice = new ProductPrice(price.ProductId, GetCostWithDiscount(price), price.Discount, price.Cost,
            price.Date);
        return Result.Ok(productPrice);
    }

    public async Task<Result<IEnumerable<ProductPrice>>> GetActualPrices(
        List<Guid> productIds,
        DateTime? priceDate,
        CancellationToken cancellationToken)
    {
        var prices = await priceRepository.GetPrices(productIds, priceDate, cancellationToken);

        var actualPrice = prices.Select(price =>
            new ProductPrice(price.ProductId, GetCostWithDiscount(price), price.Discount, price.Cost, price.Date));

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