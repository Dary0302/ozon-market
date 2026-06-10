using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStorageService
{
    Task<Result<Guid>> AddStorage(Storage storage, CancellationToken cancellationToken);
    
    Task<Result<Storage>> GetStorage(Guid id, CancellationToken cancellationToken);
    
    Task<Result> UpdateStorage(Guid id, Storage storage, CancellationToken cancellationToken);
    
    Task<Result> DeleteStorage(Guid id, CancellationToken cancellationToken);
}