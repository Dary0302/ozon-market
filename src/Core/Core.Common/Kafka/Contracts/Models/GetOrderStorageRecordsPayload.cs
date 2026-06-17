namespace Core.Common.Kafka.Contracts.Models;

public record GetOrderStorageRecordsPayload
{
    public bool IsAvailable { get; init; }
    public IEnumerable<DecreaseQuantity> Items { get; init; }
}