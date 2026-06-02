namespace ProductService.Domain;

public record Price(Guid ProductId, double Cost, decimal Discount) : BaseEntity
{
    public DateTime Date { get; set; } = DateTime.Now;
}