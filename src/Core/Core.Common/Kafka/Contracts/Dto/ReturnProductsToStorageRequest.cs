using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record ReturnProductsToStorageRequest : IHasCorrelationId
{
    public Guid CorrelationId { get; init; }
    public IEnumerable<ProductQuantity> Products { get; init; }
}