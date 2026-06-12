namespace Core.Common.Kafka.Contracts;

public record KafkaError
{
    public string Code { get; init; }
    public string Message { get; init; }
    public string? Details { get; init; }
}