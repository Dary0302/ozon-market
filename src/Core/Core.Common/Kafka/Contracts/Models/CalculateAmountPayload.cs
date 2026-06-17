namespace Core.Common.Kafka.Contracts.Models;

public record CalculateAmountPayload
{
    public bool IsAvailable { get; init; }
    public decimal Amount { get; init; }
}