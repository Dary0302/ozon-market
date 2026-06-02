using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IPvzRepository
{
    Task Add(Pvz pvz);

    Task<Pvz?> Get(Guid id);
    
    Task<IEnumerable<Pvz>> GetAll();
    
    Task Update(Pvz pvz);
    
    Task Delete(Guid id);
}