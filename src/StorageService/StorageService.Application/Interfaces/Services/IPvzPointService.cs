using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzPointService
{
    Task<Result<Guid>> AddPvzPoint(AddPvzPointDto addPvzPointDto, CancellationToken cancellationToken);
    
    Task<Result<PvzPoint>> GetPvzPoint(Guid id, CancellationToken cancellationToken);
    
    Task<Result> UpdatePvzPoint(Guid id, AddPvzPointDto addPvzPointDto, CancellationToken cancellationToken);
    
    Task<Result> DeletePvzPoint(Guid id, CancellationToken cancellationToken);
}