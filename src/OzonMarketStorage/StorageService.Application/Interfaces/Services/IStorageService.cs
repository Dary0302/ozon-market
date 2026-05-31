using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStorageService
{
    public Task<Result<Guid>> AddStorage(Storage storage);
    
    public Task<Result<Storage>> GetStorage(Guid id);
    
    public Task<Result<bool>> UpdateStorage(Guid id, Storage storage);
    
    public Task<Result<bool>> DeleteStorage(Guid id);
}