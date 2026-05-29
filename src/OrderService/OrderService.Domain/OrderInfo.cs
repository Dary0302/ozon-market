using OrderService.Domain;

namespace OrderService.Domain;

public record OrderInfo(Order Order, List<OrderItem> OrderItems);