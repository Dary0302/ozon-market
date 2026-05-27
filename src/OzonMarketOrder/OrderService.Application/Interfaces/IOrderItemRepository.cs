using FluentResults;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<Guid> Add(OrderItem orderItem);
    
    Task<List<OrderItem>> GetAllByOrderId(Guid orderId);
}