using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Dto;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class StoragePointService(IStoragePointRepository storagePointRepository) : IStoragePointService
{
    private const string NotFoundExceptionMessage = "Местоположение склада не найдено";
    
    public async Task<Result<Guid>> AddStoragePoint(AddStoragePointDto addStoragePointDto, CancellationToken cancellationToken)
    {
        var storagePoint = new StoragePoint
        {
            Id = addStoragePointDto.Id,
            StorageId =  addStoragePointDto.StorageId,
            Longitude = addStoragePointDto.Longitude,
            Latitude = addStoragePointDto.Latitude,
        };
        
        await storagePointRepository.Add(storagePoint, cancellationToken);
        
        return Result.Ok(storagePoint.Id);
    }

    public async Task<Result<StoragePoint>> GetStoragePoint(Guid id, CancellationToken cancellationToken)
    {
        var existingStoragePoint = await storagePointRepository.Get(id, cancellationToken);

        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        return Result.Ok(existingStoragePoint);
    }

    public async Task<Result> UpdateStoragePoint(Guid id, AddStoragePointDto addStoragePointDto, CancellationToken cancellationToken)
    {
        var existingStoragePoint = await storagePointRepository.Get(id, cancellationToken);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        var storagePoint = new StoragePoint
        {
            Id = addStoragePointDto.Id,
            StorageId =  addStoragePointDto.StorageId,
            Longitude = addStoragePointDto.Longitude,
            Latitude = addStoragePointDto.Latitude,
        };

        await storagePointRepository.Update(storagePoint, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeleteStoragePoint(Guid id, CancellationToken cancellationToken)
    {
        var existingStoragePoint = await storagePointRepository.Get(id, cancellationToken);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await storagePointRepository.Delete(id, cancellationToken);
        
        return Result.Ok();
    }
}