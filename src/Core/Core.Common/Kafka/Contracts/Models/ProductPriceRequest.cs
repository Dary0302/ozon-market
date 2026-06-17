namespace Core.Common.Kafka.Contracts.Models;

public record ProductPriceRequest(DateTime Date, IEnumerable<Guid> ProductIds);