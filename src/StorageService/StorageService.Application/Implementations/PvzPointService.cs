using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Dto;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class PvzPointService(IPvzPointRepository pvzPointRepository) : IPvzPointService
{
    private const string NotFoundExceptionMessage = "Местоположение пункта выдачи заказов не найдено";
    
    public async Task<Result<Guid>> AddPvzPoint(AddPvzPointDto addPvzPointDto, CancellationToken cancellationToken)
    {
        var pvzPoint = new PvzPoint {
            Id = addPvzPointDto.Id, 
            PvzId = addPvzPointDto.PvzId, 
            Longitude = addPvzPointDto.Longitude, 
            Latitude = addPvzPointDto.Latitude
        };
        
        await pvzPointRepository.Add(pvzPoint, cancellationToken);
        
        return Result.Ok(pvzPoint.Id);
    }

    public async Task<Result<PvzPoint>> GetPvzPoint(Guid id, CancellationToken cancellationToken)
    {
        var existingPvzPoint = await pvzPointRepository.Get(id, cancellationToken);

        if (existingPvzPoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        return Result.Ok(existingPvzPoint);
    }

    public async Task<Result> UpdatePvzPoint(Guid id, AddPvzPointDto addPvzPointDto, CancellationToken cancellationToken)
    {
        var existingStoragePoint = await pvzPointRepository.Get(id, cancellationToken);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        var pvzPoint = new PvzPoint {
            Id = addPvzPointDto.Id, 
            PvzId = addPvzPointDto.PvzId, 
            Longitude = addPvzPointDto.Longitude, 
            Latitude = addPvzPointDto.Latitude
        };

        await pvzPointRepository.Update(pvzPoint, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeletePvzPoint(Guid id, CancellationToken cancellationToken)
    {
        var existingPvzPoint = await pvzPointRepository.Get(id, cancellationToken);
        
        if (existingPvzPoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await pvzPointRepository.Delete(id, cancellationToken);
        
        return Result.Ok();
    }
}