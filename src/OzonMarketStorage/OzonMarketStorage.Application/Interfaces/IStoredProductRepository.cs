using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Application.Interfaces;

public interface IStoredProductRepository
{
    Task Add(StoredProduct storedProduct);

    Task<StoredProduct> Get(Guid id);
    
    Task Update(StoredProduct storedProduct);
    
    Task Delete(Guid id);
}