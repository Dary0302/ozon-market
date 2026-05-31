using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStoragePointRepository
{
    Task Add(StoragePoint storagePoint);

    Task<StoragePoint?> Get(Guid id);
    
    Task<IEnumerable<StoragePoint>> GetStoragePoints(IEnumerable<Guid> storageIds);
    
    Task Update(StoragePoint storagePoint);
    
    Task Delete(Guid id);
}