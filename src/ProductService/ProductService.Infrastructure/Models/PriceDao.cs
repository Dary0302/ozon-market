namespace ProductService.Infrastructure.Models;

public record PriceDao : BaseEntityDao
{
    public Guid ProductId { get; init; }
    public DateTime Date { get; init; }
    public double Cost { get; init; }
    public int Discount { get; init; }
}