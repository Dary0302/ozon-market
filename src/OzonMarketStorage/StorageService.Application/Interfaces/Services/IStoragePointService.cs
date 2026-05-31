using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoragePointService
{
    public Task<Result<Guid>> AddStoragePoint(StoragePoint storagePoint);
    
    public Task<Result<StoragePoint>> GetStoragePoint(Guid id);
    
    public Task<Result> UpdateStoragePoint(Guid id, StoragePoint storagePoint);
    
    public Task<Result> DeleteStoragePoint(Guid id);
}