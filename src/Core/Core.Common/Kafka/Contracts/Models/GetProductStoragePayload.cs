namespace Core.Common.Kafka.Contracts.Models;

public record GetProductStoragePayload
{
    public bool IsAvailable { get; init; }
    public IEnumerable<ProductStorage> Items { get; init; }
}