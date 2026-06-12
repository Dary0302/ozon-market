using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record ReduceCountOfProductsRequest : IHasCorrelationId
{
    public Guid CorrelationId { get; init; }
    public IEnumerable<DecreaseQuantity> Items { get; init; }
}