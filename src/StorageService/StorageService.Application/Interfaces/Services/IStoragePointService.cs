using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoragePointService
{
    Task<Result<Guid>> AddStoragePoint(AddStoragePointDto addStoragePointDto, CancellationToken cancellationToken);
    
    Task<Result<StoragePoint>> GetStoragePoint(Guid id, CancellationToken cancellationToken);
    
    Task<Result> UpdateStoragePoint(Guid id, AddStoragePointDto addStoragePointDto, CancellationToken cancellationToken);
    
    Task<Result> DeleteStoragePoint(Guid id, CancellationToken cancellationToken);
}