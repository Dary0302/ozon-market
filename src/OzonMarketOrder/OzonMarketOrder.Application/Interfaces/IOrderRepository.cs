using FluentResults;
using OzonMarketOrder.Application.Models;
using OzonMarketOrder.Domain;

namespace OzonMarketOrder.Application.Interfaces;

public interface IOrderRepository
{
    Task<Result<Guid>> Create(Order order);
    
    Task<Result<Order>> GetById(Guid id);
    
    Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize);
    
    Task<Result<Guid>> UpdateStatus(Guid id, Status status);
    
    Task<Result> Delete(Guid id);
}