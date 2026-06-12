using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts.Dto;

public record KafkaResponse<T> : IHasCorrelationId
{
    public Guid CorrelationId { get; init; }
    public KafkaResponse<T> Result { get; init; }
}