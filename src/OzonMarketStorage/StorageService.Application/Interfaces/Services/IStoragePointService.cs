using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoragePointService
{
    public Task<Result<Guid>> AddStoragePoint(StoragePoint storagePoint);
    
    public Task<Result<StoragePoint>> GetStoragePoint(Guid id);
    
    public Task<Result<bool>> UpdateStoragePoint(Guid id, StoragePoint storagePoint);
    
    public Task<Result<bool>> DeleteStoragePoint(Guid id);
}