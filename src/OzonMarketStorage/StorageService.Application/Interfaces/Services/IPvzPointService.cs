using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzPointService
{
    public Task<Result<Guid>> AddPvzPoint(PvzPoint pvzPoint);
    
    public Task<Result<PvzPoint>> GetPvzPoint(Guid id);
    
    public Task<Result<bool>> UpdatePvzPoint(Guid id, PvzPoint pvzPoint);
    
    public Task<Result<bool>> DeletePvzPoint(Guid id);
}