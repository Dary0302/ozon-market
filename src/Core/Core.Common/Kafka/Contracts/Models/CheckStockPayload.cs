namespace Core.Common.Kafka.Contracts.Models;

public record CheckStockPayload
{
    public bool IsAvailable { get; init; }
    public IEnumerable<StockCheckResult> Result { get; init; }
}