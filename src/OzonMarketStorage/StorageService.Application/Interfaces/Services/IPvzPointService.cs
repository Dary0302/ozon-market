using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzPointService
{
    public Task<Result<Guid>> AddPvzPoint(PvzPoint pvzPoint);
    
    public Task<Result<PvzPoint>> GetPvzPoint(Guid id);
    
    public Task<Result> UpdatePvzPoint(Guid id, PvzPoint pvzPoint);
    
    public Task<Result> DeletePvzPoint(Guid id);
}