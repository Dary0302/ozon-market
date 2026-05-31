using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoredProductService
{
    Task<Result<bool>> AddStoredProduct(StoredProduct storedProduct);
    
    Task<Result<IEnumerable<ProductQuantity>>> GetStoredProducts();
    
    Task<Result<List<StockCheckResult>>> CheckStock(List<ProductQuantity> orderedProducts);

    Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts);
    
    Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts);
    
    Task<Result> IncreaseStoredProductQuantity(List<IncreaseQuantity> arrivedProducts);
}