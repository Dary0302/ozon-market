using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStorageService
{
    Task<Result<Guid>> AddStorage(AddStorageDto addStorageDto, CancellationToken cancellationToken);
    
    Task<Result<Storage>> GetStorage(Guid id, CancellationToken cancellationToken);
    
    Task<Result> UpdateStorage(Guid id, AddStorageDto addStorageDto, CancellationToken cancellationToken);
    
    Task<Result> DeleteStorage(Guid id, CancellationToken cancellationToken);
}