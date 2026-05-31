using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStoredProductRepository
{
    Task Add(StoredProduct storedProduct);

    Task<IEnumerable<StoredProduct>> GetByOrderedProducts(List<DecreaseQuantity> orderedProducts);
    
    Task<IEnumerable<ProductQuantity>> GetAllInStock();
    
    Task<IEnumerable<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds);

    Task<IEnumerable<StoredProduct>> GetProductsStorages(List<ProductQuantity> orderedProducts);
    
    Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts);
    
    Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts);
    
    Task Delete(Guid id);
}