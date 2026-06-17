using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStorageRepository
{
    Task Add(Storage storage, CancellationToken cancellationToken);

    Task<Storage?> Get(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Storage>> GetAll(CancellationToken cancellationToken);
    
    Task Update(Storage storage, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}