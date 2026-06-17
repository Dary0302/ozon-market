namespace Core.Common.Kafka.Contracts.Models;

public record GetProductsPricePayload
{
    public bool IsAvailable { get; init; }
    public IEnumerable<ProductPrice> ProductPrices { get; init; }
}