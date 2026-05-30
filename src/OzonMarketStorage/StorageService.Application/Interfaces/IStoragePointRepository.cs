using StorageService.Domain;

namespace StorageService.Application.Interfaces;

public interface IStoragePointRepository
{
    Task Add(StoragePoint storagePoint);

    Task<StoragePoint> Get(Guid id);
    
    Task Update(StoragePoint storagePoint);
    
    Task Delete(Guid id);
}