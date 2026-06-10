using FluentResults;
using StorageService.Application.Dto;
using StorageService.Domain;

namespace StorageService.Application.Interfaces.Services;

public interface IStoredProductService
{
    Task<Result> AddStoredProduct(StoredProduct storedProduct, CancellationToken cancellationToken);
    
    Task<Result<IEnumerable<ProductQuantity>>> GetStoredProductsInStock(CancellationToken cancellationToken);
    
    Task<Result<List<StockCheckResult>>> CheckStock(List<ProductQuantity> orderedProducts, CancellationToken cancellationToken);

    Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts, CancellationToken cancellationToken);

    Task<Result<List<DecreaseQuantity>>> GetOrderStoragesRecords(Guid pvzId, List<ProductQuantity> orderedProducts, CancellationToken cancellationToken);
    
    Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken);
    
    Task<Result> IncreaseStoredProductQuantity(List<IncreaseQuantity> arrivedProducts, CancellationToken cancellationToken);
}