using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzPointService
{
    Task<Result<Guid>> AddPvzPoint(PvzPoint pvzPoint, CancellationToken cancellationToken);
    
    Task<Result<PvzPoint>> GetPvzPoint(Guid id, CancellationToken cancellationToken);
    
    Task<Result> UpdatePvzPoint(Guid id, PvzPoint pvzPoint, CancellationToken cancellationToken);
    
    Task<Result> DeletePvzPoint(Guid id, CancellationToken cancellationToken);
}