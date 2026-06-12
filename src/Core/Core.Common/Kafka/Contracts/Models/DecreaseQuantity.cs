namespace Core.Common.Kafka.Contracts.Models;

public record DecreaseQuantity(Guid ProductId, Guid StorageId, int Quantity);