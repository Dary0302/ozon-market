using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record GetProductsPriceRequest : IHasCorrelationId
{
    public Guid CorrelationId { get; init; }
    public IEnumerable<ProductPriceRequest> Requests { get; init; }
}