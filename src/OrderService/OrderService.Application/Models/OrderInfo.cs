using OrderService.Domain;

namespace OrderService.Application.Models;

public record OrderInfo(Order Order, List<OrderItem> OrderItems);