using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Dto;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class StorageService(IStorageRepository storageRepository) : IStorageService
{
    private const string NotFoundExceptionMessage = "Склад не найден";
    
    public async Task<Result<Guid>> AddStorage(Storage storage, CancellationToken cancellationToken)
    {
        await storageRepository.Add(storage, cancellationToken);
        
        return Result.Ok(storage.Id);
    }

    public async Task<Result<Storage>> GetStorage(Guid id, CancellationToken cancellationToken)
    {
        var existingStorage = await storageRepository.Get(id, cancellationToken);

        if (existingStorage is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        return Result.Ok(existingStorage);
    }

    public async Task<Result> UpdateStorage(Guid id, Storage storage, CancellationToken cancellationToken)
    {
        var existingStorage = await storageRepository.Get(id, cancellationToken);
        
        if (existingStorage is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await storageRepository.Update(storage, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteStorage(Guid id, CancellationToken cancellationToken)
    {
        var existingStorage = await storageRepository.Get(id, cancellationToken);
        
        if (existingStorage is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await storageRepository.Delete(id, cancellationToken);
        
        return Result.Ok();
    }
}