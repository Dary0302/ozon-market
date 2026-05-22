namespace OzonMarketOrder.Infrastructure.Models;

public class OrderItemDao
{
    public Guid OderId { get; init; }
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}