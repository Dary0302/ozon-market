namespace Core.Common.Kafka.Contracts.Models;

public record StockCheckResult(Guid ProductId, int Difference);