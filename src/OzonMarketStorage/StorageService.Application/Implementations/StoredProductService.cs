using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class StoredProductService(
    IStoredProductRepository storedProductRepository,
    IStoragePointRepository storagePointRepository,
    IPvzPointRepository pvzPointRepository) : IStoredProductService
{
    public async Task<Result<bool>> AddStoredProduct(StoredProduct storedProduct)
    {
        await storedProductRepository.Add(storedProduct);
        
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<ProductQuantity>>> GetStoredProducts()
    {
        var storedProducts = await storedProductRepository.GetAllInStock();

        return Result.Ok(storedProducts);
    }

    public async Task<Result<List<StockCheckResult>>> CheckStock(List<ProductQuantity> orderedProducts)
    {
        var productIds = orderedProducts.Select(product => product.ProductId);
        
        var storedProducts = await storedProductRepository.GetProductsQuantity(productIds);

        if (storedProducts.Count != orderedProducts.Count)
        {
            return Result.Fail(AppError.Validation("Количество заказанных товаров и товаров на складе не совпадает"));
        }

        var stockCheckResults = CheckStock(storedProducts, orderedProducts);
        
        if (stockCheckResults.Any(result => result.Difference < 0))
        {
            return Result.Fail(AppError.Validation("Товара на складе не хватает"));
        }

        return Result.Ok(stockCheckResults);
    }

    public async Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts)
    {
        var productIds = orderedProducts.Select(product => product.ProductId);
        var storedProducts = await storedProductRepository.GetProductsStorages(productIds);
        var finalCount = storedProducts.Select(product => product.ProductId).Distinct().Count();
        
        if (finalCount != orderedProducts.Count())
        {
            return Result.Fail(AppError.Validation("Количество заказанных товаров и товаров на складе не совпадает"));
        }
        
        var pvzPoint =  await pvzPointRepository.Get(pvzId);

        if (pvzPoint is null)
        {
            return Result.Fail(AppError.NotFound("Пвз не найден"));
        }
        
        var suitableStorages = GetSuitableStorages(storedProducts, orderedProducts);

        var storageIds = suitableStorages.Select(product => product.StorageId);
        
        var storagePoints = (await storagePointRepository.GetStoragePoints(storageIds)).ToList();
        
        var orderStorages = ChooseOrderStorages(suitableStorages, storagePoints, pvzPoint);
        
        var farthestStorageDistance = orderStorages.Max(product => product.Distance);

        const double scale = 100;
        const double shiftDuration = 12;
        const double averageSpeed = 70;

        var travelTime = farthestStorageDistance * scale / (averageSpeed * shiftDuration);
        var deliveryTime = DateTime.Now.AddHours(travelTime);
        
        return Result.Ok(deliveryTime);
    }

    public async Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts)
    {
        var storedProducts = await storedProductRepository.GetByOrderedProducts(orderedProducts);

        if (storedProducts.Any(product => product.Quantity < 0))
        {
            return Result.Fail(AppError.Validation("Не хватает товара на складе"));
        }

        await storedProductRepository.DecreaseCount(orderedProducts);
        
        return Result.Ok();
    }

    public async Task<Result> IncreaseStoredProductQuantity(List<IncreaseQuantity> arrivedProducts)
    {
        await storedProductRepository.IncreaseCount(arrivedProducts);
        
        return Result.Ok();
    }

    /// <summary>
    /// Возвращает коллекцию с разницей товаров на складах и заказанных товаров
    /// </summary>
    private List<StockCheckResult> CheckStock(List<ProductQuantity> storedProducts,  List<ProductQuantity> orderedProducts)
    {
        var stockCheckResults = storedProducts.Join(orderedProducts,
            storedProduct => storedProduct.ProductId,
            orderedProduct => orderedProduct.ProductId,
            (storedProduct, orderedProduct) => new StockCheckResult {
                ProductId = storedProduct.ProductId,
                Difference = storedProduct.Quantity - orderedProduct.Quantity,
            }).ToList();

        return stockCheckResults;
    }
    
    /// <summary>
    /// Возвращает список из объектов с ProductId, StorageId, Distance до указанного пункта выдачи заказов
    /// </summary>
    private static List<OrderStorage> ChooseOrderStorages(
        List<StoredProduct> storedProducts, 
        List<StoragePoint> storagePoints, 
        PvzPoint pvzPoint)
    {
        var orderStorages = storedProducts.Join(storagePoints,
                product => product.StorageId,
                storagePoint => storagePoint.StorageId,
                (product, storagePoint) => new OrderStorage
                (
                    product.ProductId,
                    storagePoint.StorageId,
                    Math.Sqrt(Math.Pow(storagePoint.Longitude - pvzPoint.Longitude, 2) + 
                                      Math.Pow(storagePoint.Latitude - pvzPoint.Latitude, 2))
                ))
            .GroupBy(storage => storage.ProductId)
            .Select(group => group.OrderBy(storage => storage.Distance).First()) 
            .ToList();
        
        return orderStorages;
    }

    /// <summary>
    /// Возвращает список из складов, в которых достаточно продуктов для заказа
    /// </summary>
    private static List<StoredProduct> GetSuitableStorages(List<StoredProduct> storedProducts, List<ProductQuantity> orderedProducts)
    {
        var suitableStorages = storedProducts.Join(orderedProducts,
                storedProduct => storedProduct.ProductId,
                orderedProduct => orderedProduct.ProductId,
                (storedProduct, orderedProduct) => new
                {
                    ProductId = storedProduct.ProductId,
                    StorageId = storedProduct.StorageId,
                    StoredQuantity = storedProduct.Quantity,
                    OrderedQuantity = orderedProduct.Quantity
                })
            .Where(storedProduct => storedProduct.StoredQuantity >= storedProduct.OrderedQuantity)
            .Select(product => new StoredProduct(product.ProductId, product.StorageId, product.StoredQuantity));
        
        return suitableStorages.ToList();
    }
}