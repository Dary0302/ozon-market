using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class PvzPointService(IPvzPointRepository pvzPointRepository) : IPvzPointService
{
    private const string NotFoundExceptionMessage = "Местоположение пункта выдачи заказов не найдено";
    
    public async Task<Result<Guid>> AddPvzPoint(PvzPoint pvzPoint)
    {
        await pvzPointRepository.Add(pvzPoint);
        
        return Result.Ok(pvzPoint.Id);
    }

    public async Task<Result<PvzPoint>> GetPvzPoint(Guid id)
    {
        var existingPvzPoint = await pvzPointRepository.Get(id);

        if (existingPvzPoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        return Result.Ok(existingPvzPoint);
    }

    public async Task<Result> UpdatePvzPoint(Guid id, PvzPoint pvzPoint)
    {
        var existingStoragePoint = await pvzPointRepository.Get(id);
        
        if (existingStoragePoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }

        await pvzPointRepository.Update(pvzPoint);
        
        return Result.Ok();
    }

    public async Task<Result> DeletePvzPoint(Guid id)
    {
        var existingPvzPoint = await pvzPointRepository.Get(id);
        
        if (existingPvzPoint is null)
        {
            return Result.Fail(AppError.NotFound(NotFoundExceptionMessage));
        }
        
        await pvzPointRepository.Delete(id);
        
        return Result.Ok();
    }
}