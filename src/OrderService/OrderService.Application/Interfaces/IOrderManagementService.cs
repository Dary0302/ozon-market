using Core.Common.Kafka.Contracts.Models;
using FluentResults;
using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderManagementService
{
    Task<Result<Guid>> Create(Guid pvzId, decimal amount, 
        IEnumerable<ProductQuantity> productIds, CancellationToken cancellationToken);
    
    Task<Result<Order>> GetById(Guid id, CancellationToken cancellationToken);
    
    Task<Result<PagedResult<Order>>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken);
    
    Task<Result<Guid>> UpdateStatus(Guid id, Status status, CancellationToken cancellationToken);
    
    Task<Result> Delete(Guid id, CancellationToken cancellationToken);
    
    Task<Result<OrderInfoWithPrice>> GetInfoById(Guid id, CancellationToken cancellationToken);
    
    Task<Result<PagedResult<OrderInfoWithPrice>>> GetAllInfo(int pageNumber, int pageSize, CancellationToken cancellationToken);
    
    Task<Result> Cancel(Guid id, CancellationToken cancellationToken);
}