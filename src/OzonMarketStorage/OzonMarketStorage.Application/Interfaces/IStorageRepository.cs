using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Application.Interfaces;

public interface IStorageRepository
{
    Task Add(Storage storage);

    Task<Storage> Get(Guid id);
    
    Task Update(Storage storage);
    
    Task Delete(Guid id);
}