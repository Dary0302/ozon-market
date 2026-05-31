using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class StoragePointService(IStoragePointRepository storagePointRepository) : IStoragePointService
{
    public async Task<Result<Guid>> AddStoragePoint(StoragePoint storagePoint)
    {
        await storagePointRepository.Add(storagePoint);
        
        return Result.Ok(storagePoint.Id);
    }

    public async Task<Result<StoragePoint>> GetStoragePoint(Guid id)
    {
        var existingStoragePoint = await storagePointRepository.Get(id);

        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound("Местоположение склада не найдено"));
        }
        
        return Result.Ok(existingStoragePoint);
    }

    public async Task<Result> UpdateStoragePoint(Guid id, StoragePoint storagePoint)
    {
        var existingStoragePoint = await storagePointRepository.Get(id);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound("Местоположение склада не найдено"));
        }

        await storagePointRepository.Update(storagePoint);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteStoragePoint(Guid id)
    {
        var existingStoragePoint = await storagePointRepository.Get(id);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound("Местоположение склада не найдено"));
        }
        
        await storagePointRepository.Delete(id);
        
        return Result.Ok();
    }
}