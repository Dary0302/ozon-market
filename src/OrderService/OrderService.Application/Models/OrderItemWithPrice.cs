namespace OrderService.Application.Models;

public record OrderItemWithPrice(
    Guid ProductId,
    int Quantity,
    Decimal Price
);