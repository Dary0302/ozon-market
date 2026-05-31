using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class PvzService(IPvzRepository pvzRepository) : IPvzService
{
    public async Task<Result<Guid>> AddPvz(Pvz pvz)
    {
        await pvzRepository.Add(pvz);
        
        return Result.Ok(pvz.Id);
    }

    public async Task<Result<Pvz>> GetPvz(Guid id)
    {
        var existingPvz = await pvzRepository.Get(id);

        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound("Пункт выдачи заказов не найден"));
        }
        
        return Result.Ok(existingPvz);
    }

    public async Task<Result<IEnumerable<Pvz>>> GetAllPvz()
    {
        var allPvz =  await pvzRepository.GetAll();
        
        return Result.Ok(allPvz);
    }

    public async Task<Result<bool>> UpdatePvz(Guid id, Pvz pvz)
    {
        var existingPvz = await pvzRepository.Get(id);
        
        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound("Пункт выдачи заказов не найден"));
        }

        await pvzRepository.Update(pvz);
        
        return Result.Ok();
    }

    public async Task<Result<bool>> DeletePvz(Guid id)
    {
        var existingPvz = await pvzRepository.Get(id);
        
        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound("Пункт выдачи заказов не найден"));
        }
        
        await pvzRepository.Delete(id);
        
        return Result.Ok();
    }
}