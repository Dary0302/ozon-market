using System.Data;
using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Guid> Create(Order order, IDbConnection dbConnection, IDbTransaction dbTransaction);
    
    Task<Order?> GetById(Guid id);
    
    Task<PagedResult<Order>> GetAll(int pageNumber, int pageSize);
    
    Task<Guid> Save(Order order, IDbConnection dbConnection, IDbTransaction dbTransaction);
    
    Task Delete(Guid id, IDbConnection dbConnection, IDbTransaction dbTransaction);
}