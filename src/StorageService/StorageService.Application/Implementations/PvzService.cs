using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Dto;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class PvzService(IPvzRepository pvzRepository, CancellationToken cancellationToken) : IPvzService
{
    private const string NotFounExceptionMessage = "Пункт выдачи заказов не найден";
    
    public async Task<Result<Guid>> AddPvz(AddPvzDto addPvzDto, CancellationToken cancellationToken)
    {
        var pvz = new Pvz
        {
            Id = addPvzDto.Id,
            Address = addPvzDto.Address,
            PointId = addPvzDto.PointId,
        };
        
        await pvzRepository.Add(pvz, cancellationToken);
        
        return Result.Ok(pvz.Id);
    }

    public async Task<Result<Pvz>> GetPvz(Guid id, CancellationToken cancellationToken)
    {
        var existingPvz = await pvzRepository.Get(id, cancellationToken);

        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound(NotFounExceptionMessage));
        }
        
        return Result.Ok(existingPvz);
    }

    public async Task<Result<IEnumerable<Pvz>>> GetAllPvz(CancellationToken cancellationToken)
    {
        var allPvz =  await pvzRepository.GetAll(cancellationToken);
        
        return Result.Ok(allPvz);
    }

    public async Task<Result> UpdatePvz(Guid id, AddPvzDto addPvzDto, CancellationToken cancellationToken)
    {
        var existingPvz = await pvzRepository.Get(id, cancellationToken);
        
        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound(NotFounExceptionMessage));
        }
        
        var pvz = new Pvz
        {
            Id = addPvzDto.Id,
            Address = addPvzDto.Address,
            PointId = addPvzDto.PointId,
        };

        await pvzRepository.Update(pvz, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> DeletePvz(Guid id, CancellationToken cancellationToken)
    {
        var existingPvz = await pvzRepository.Get(id, cancellationToken);
        
        if (existingPvz is null)
        {
            return Result.Fail(AppError.NotFound(NotFounExceptionMessage));
        }
        
        await pvzRepository.Delete(id, cancellationToken);
        
        return Result.Ok();
    }
}