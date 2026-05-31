using FluentResults;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoredProductService
{
    public Task<Result<bool>> AddStoredProduct(StoredProduct storedProduct);
    
    public Task<Result<IEnumerable<ProductQuantity>>> GetStoredProducts();
    
    public Task<Result<List<StockCheckResult>>> CheckStock(List<ProductQuantity> orderedProducts);

    public Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts);
    
    public Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts);
    
    public Task<Result> IncreaseStoredProductQuantity(List<IncreaseQuantity> arrivedProducts);
}