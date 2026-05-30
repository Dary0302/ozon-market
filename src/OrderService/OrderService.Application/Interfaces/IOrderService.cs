using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderService
{
    Task<Result<Guid>> Create(Guid pvzId, decimal amount, List<ProductQuantity> productIds);
    
    Task<Result<Order?>> GetById(Guid id);
    
    Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize);
    
    Task<Result<Guid>> UpdateStatus(Guid id, Status status);
    
    Task Delete(Guid id);
    
    Task<Result<OrderInfo>> GetInfoById(Guid id);
    
    Task<Result<PagedResult<OrderInfo>>> GetAllInfo(int pageNumber, int pageSize);
}