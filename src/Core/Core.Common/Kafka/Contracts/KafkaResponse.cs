using Core.Common.Kafka.Interfaces;

namespace Core.Common.Kafka.Contracts;

public record KafkaResponse<T> : IHasCorrelationId
{
    public Guid CorrelationId { get; init; }
    public bool IsSuccess { get; init; }
    public T? Payload { get; init; }
    public IReadOnlyList<KafkaError> Errors { get; init; } = [];

    public static KafkaResponse<T> Success(Guid correlationId, T payload) =>
        new() { CorrelationId = correlationId, IsSuccess = true, Payload = payload };

    public static KafkaResponse<T> Failure(Guid correlationId, IEnumerable<KafkaError> errors) =>
        new() { CorrelationId = correlationId, IsSuccess = false, Errors = errors.ToList() };

    public static KafkaResponse<T> Failure(Guid correlationId, string code, string message) =>
        Failure(correlationId, [new KafkaError { Code = code, Message = message }]);
}