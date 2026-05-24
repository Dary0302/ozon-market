using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Result<Guid>> Create(Order order);
    
    Task<Result<Order>> GetById(Guid id);
    
    Task<Result<OrdersPagedResult<Order>>> GetAll(int pageNumber, int pageSize);
    
    Task<Result<Guid>> UpdateStatus(Guid id, Status status);
    
    Task<Result> Delete(Guid id);
}