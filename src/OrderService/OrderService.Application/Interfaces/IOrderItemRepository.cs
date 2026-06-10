using System.Data;
using FluentResults;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderItemRepository
{
    Task<Guid> Add(List<OrderItem> orderItems,
        IDbConnection dbConnection, 
        IDbTransaction dbTransaction, 
        CancellationToken cancellationToken);
    
    Task<IEnumerable<OrderItem>> GetAllByOrderId(Guid orderId, CancellationToken cancellationToken);
}