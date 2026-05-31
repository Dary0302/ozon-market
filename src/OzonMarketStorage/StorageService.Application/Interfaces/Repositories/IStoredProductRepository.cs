using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStoredProductRepository
{
    Task Add(StoredProduct storedProduct);

    Task<IEnumerable<StoredProduct>> GetByOrderedProducts(List<DecreaseQuantity> orderedProducts);
    
    Task<IEnumerable<ProductQuantity>> GetAllInStock();
    
    Task<List<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds);

    Task<List<StoredProduct>> GetProductsStorages(IEnumerable<Guid> productIds);
    
    Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts);
    
    Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts);
    
    Task Delete(Guid id);
}