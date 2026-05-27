namespace OrderService.Domain;

public record OrderItem(Guid OrderId, Guid ProductId, int Quantity);