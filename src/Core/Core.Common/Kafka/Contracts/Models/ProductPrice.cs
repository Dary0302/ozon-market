namespace Core.Common.Kafka.Contracts.Models;

public record ProductPrice(Guid ProductId, decimal Price, DateTime Date);