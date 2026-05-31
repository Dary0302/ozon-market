using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzPointService
{
    Task<Result<Guid>> AddPvzPoint(PvzPoint pvzPoint);
    
    Task<Result<PvzPoint>> GetPvzPoint(Guid id);
    
    Task<Result> UpdatePvzPoint(Guid id, PvzPoint pvzPoint);
    
    Task<Result> DeletePvzPoint(Guid id);
}