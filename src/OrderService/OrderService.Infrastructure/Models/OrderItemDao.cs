namespace OrderService.Infrastructure.Models;

public record OrderItemDao
{
    public Guid OderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}