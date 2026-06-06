using StorageService.Domain;

namespace StorageService.Application.Interfaces.Repositories;

public interface IStoredProductRepository
{
    Task Add(StoredProduct storedProduct, CancellationToken cancellationToken);

    Task<IEnumerable<StoredProduct>> GetByOrderedProducts(List<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken);
    
    Task<IEnumerable<ProductQuantity>> GetAllInStock(CancellationToken cancellationToken);
    
    Task<IEnumerable<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds, CancellationToken cancellationToken);

    Task<IEnumerable<StoredProduct>> GetProductsStorages(List<ProductQuantity> orderedProducts, CancellationToken cancellationToken);
    
    Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken);
    
    Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts, CancellationToken cancellationToken);
    
    Task Delete(Guid id, CancellationToken cancellationToken);
}