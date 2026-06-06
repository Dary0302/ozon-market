using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzService
{
    Task<Result<Guid>> AddPvz(Pvz pvz);
    
    Task<Result<Pvz>> GetPvz(Guid id);
    
    Task<Result<IEnumerable<Pvz>>> GetAllPvz();
    
    Task<Result> UpdatePvz(Guid id, Pvz pvz);
    
    Task<Result> DeletePvz(Guid id);
}