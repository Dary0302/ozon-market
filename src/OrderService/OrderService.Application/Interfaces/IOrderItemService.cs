using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderItemService
{
    Task<Guid> Add(OrderItem orderItem);
    
    Task<List<OrderItem>> GetAllByOrderId(Guid orderId);
}