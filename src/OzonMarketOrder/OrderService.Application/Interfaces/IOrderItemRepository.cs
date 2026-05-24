using FluentResults;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<Result<Guid>> Add(OrderItem orderItem);
    
    Task<Result<List<OrderItem>>> GetAllByOrderId(Guid orderId);
}