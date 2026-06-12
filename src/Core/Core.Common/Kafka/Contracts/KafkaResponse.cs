namespace Core.Common.Kafka.Contracts;

public record KafkaResponse<T>
{
    public bool IsSuccess { get; init; }
    public T? Payload { get; init; }
    public IReadOnlyList<KafkaError> Errors { get; init; } = [];
    
    public static KafkaResponse<T> Success(T payload) =>
        new() { IsSuccess = true, Payload = payload };

    public static KafkaResponse<T> Failure(IEnumerable<KafkaError> errors) =>
        new() { IsSuccess = false, Errors = errors.ToList() };

    public static KafkaResponse<T> Failure(string code, string message) =>
        Failure([new KafkaError { Code = code, Message = message }]);
}