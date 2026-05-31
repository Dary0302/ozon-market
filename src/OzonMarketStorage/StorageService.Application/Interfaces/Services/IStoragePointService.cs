using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoragePointService
{
    Task<Result<Guid>> AddStoragePoint(StoragePoint storagePoint);
    
    Task<Result<StoragePoint>> GetStoragePoint(Guid id);
    
    Task<Result> UpdateStoragePoint(Guid id, StoragePoint storagePoint);
    
    Task<Result> DeleteStoragePoint(Guid id);
}