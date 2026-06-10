using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStoragePointRepository
{
    Task Add(StoragePoint storagePoint, CancellationToken cancellationToken);

    Task<StoragePoint?> Get(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<StoragePoint>> GetStoragePoints(IEnumerable<Guid> storageIds, CancellationToken cancellationToken);
    
    Task Update(StoragePoint storagePoint, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}