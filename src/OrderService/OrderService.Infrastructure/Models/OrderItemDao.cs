namespace OrderService.Infrastructure.Models;

public record OrderItemDao
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}