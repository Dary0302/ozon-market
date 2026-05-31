using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IPvzService
{
    public Task<Result<Guid>> AddPvz(Pvz pvz);
    
    public Task<Result<Pvz>> GetPvz(Guid id);
    
    public Task<Result<IEnumerable<Pvz>>> GetAllPvz();
    
    public Task<Result<bool>> UpdatePvz(Guid id, Pvz storage);
    
    public Task<Result<bool>> DeletePvz(Guid id);
}