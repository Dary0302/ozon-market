using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzService
{
    Task<Result<Guid>> AddPvz(AddPvzDto addPvzDto, CancellationToken cancellationToken);
    
    Task<Result<Pvz>> GetPvz(Guid id, CancellationToken cancellationToken);
    
    Task<Result<IEnumerable<Pvz>>> GetAllPvz(CancellationToken cancellationToken);
    
    Task<Result> UpdatePvz(Guid id, AddPvzDto addPvzDto, CancellationToken cancellationToken);
    
    Task<Result> DeletePvz(Guid id, CancellationToken cancellationToken);
}