using FluentResults;
using OzonMarketOrder.Domain;

namespace OzonMarketOrder.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<Result<Guid>> Add(OrderItem orderItem);
    
    Task<Result<List<OrderItem>>> GetAllByOrderId(Guid orderId);
}