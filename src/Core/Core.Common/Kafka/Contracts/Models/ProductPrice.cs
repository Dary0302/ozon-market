namespace Core.Common.Kafka.Contracts.Models;

public record ProductPrice(Guid ProductId, decimal Cost, decimal Discount, double CostWithoutDiscount, DateTime Date);