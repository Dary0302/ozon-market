using FluentResults;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<Guid> Add(List<OrderItem> orderItems);
    
    Task<IEnumerable<OrderItem>> GetAllByOrderId(Guid orderId);
}