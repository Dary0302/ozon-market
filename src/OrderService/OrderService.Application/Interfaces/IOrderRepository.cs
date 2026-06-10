using System.Data;
using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Guid> Create(Order order, IDbConnection dbConnection, 
        IDbTransaction dbTransaction, CancellationToken cancellationToken);
    
    Task<Order?> GetById(Guid id, CancellationToken cancellationToken);
    
    Task<PagedResult<Order>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken);
    
    Task<Guid> Save(Order order, IDbConnection dbConnection, 
        IDbTransaction dbTransaction, CancellationToken cancellationToken);
    
    Task Delete(Guid id, IDbConnection dbConnection, IDbTransaction dbTransaction, CancellationToken cancellationToken);
}