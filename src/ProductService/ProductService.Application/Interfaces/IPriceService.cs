using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using ProductService.Domain;

namespace ProductService.Application.Interfaces;

public interface IPriceService
{
    Task<Result<decimal>> CalculateAmount(IEnumerable<ProductQuantity> products, CancellationToken cancellationToken);

    Task<Result<ProductPrice>> GetActualPrice(Guid productId, DateTime? priceDate, CancellationToken cancellationToken);
    
    Task<Result<IEnumerable<ProductPrice>>> GetActualPrices(List<Guid> productIds, DateTime? priceDate, CancellationToken cancellationToken);

    Task<Result> SetPrice(Price newPrice, CancellationToken cancellationToken);
}