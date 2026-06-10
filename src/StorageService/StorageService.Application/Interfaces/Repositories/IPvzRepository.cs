using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IPvzRepository
{
    Task Add(Pvz pvz, CancellationToken cancellationToken);

    Task<Pvz?> Get(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Pvz>> GetAll(CancellationToken cancellationToken);
    
    Task Update(Pvz pvz, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}