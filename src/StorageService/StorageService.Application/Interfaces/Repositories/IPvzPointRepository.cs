using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IPvzPointRepository
{
    Task Add(PvzPoint pvzPoint, CancellationToken cancellationToken);

    Task<PvzPoint?> Get(Guid id, CancellationToken cancellationToken);
    
    Task Update(PvzPoint pvzPoint, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}