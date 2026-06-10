using OrderService.Domain;

namespace OrderService.Application.Models;

public record OrderInfoWithPrice(Order Order, IEnumerable<OrderItemWithPrice> OrderItems);