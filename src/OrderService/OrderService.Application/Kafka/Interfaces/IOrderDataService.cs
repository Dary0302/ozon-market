using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using OrderService.Domain;

namespace OrderService.Application.Kafka.Interfaces;

public interface IOrderDataService
{
    Task<Result<OrderData>> GetData(
        Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken cancellationToken);
    
    Task<Result<IEnumerable<ProductPrice>>> GetPriceInfo(IEnumerable<ProductPriceRequest> requests, 
        CancellationToken cancellationToken);
}