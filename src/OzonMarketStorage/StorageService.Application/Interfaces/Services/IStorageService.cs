using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStorageService
{
    Task<Result<Guid>> AddStorage(Storage storage);
    
    Task<Result<Storage>> GetStorage(Guid id);
    
    Task<Result> UpdateStorage(Guid id, Storage storage);
    
    Task<Result> DeleteStorage(Guid id);
}