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
    private const string NotEnoughProductExceptionMessage = "Не хватает товара на складе";
    
    public async Task<Result> AddStoredProduct(StoredProduct storedProduct)
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
        
        var storedProducts = (await storedProductRepository.GetProductsQuantity(productIds)).ToList();

        if (storedProducts.Count != orderedProducts.Count)
        {
            return Result.Fail(AppError.UnprocessableContent("Количество заказанных товаров и товаров на складе не совпадает"));
        }

        var stockCheckResults = CheckStock(storedProducts, orderedProducts);
        
        if (stockCheckResults.Any(result => result.Difference < 0))
        {
            return Result.Fail(AppError.UnprocessableContent(NotEnoughProductExceptionMessage));
        }

        return Result.Ok(stockCheckResults);
    }
    
    public async Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts)
    {
        var pvzPoint =  await pvzPointRepository.Get(pvzId);

        if (pvzPoint is null)
        {
            return Result.Fail(AppError.NotFound("Пвз не найден"));
        }

        var result = await GetOrderStorages(orderedProducts);
        var storedProducts = result.Item1;
        var storagePoints = result.Item2;
        
        var chosenStorages = ChooseOrderStorages(storedProducts, storagePoints, pvzPoint);

        var farthestStorageDistance = chosenStorages.Max(product => product.Distance);

        var deliveryTime = CalculateDeliveryTime(farthestStorageDistance);

        return Result.Ok(deliveryTime);
    }

    public async Task<Result<List<DecreaseQuantity>>> GetOrderStoragesRecords(Guid pvzId, List<ProductQuantity> orderedProducts)
    {
        var pvzPoint =  await pvzPointRepository.Get(pvzId);

        if (pvzPoint is null)
        {
            return Result.Fail(AppError.NotFound("Пвз не найден"));
        }
        
        var result = await GetOrderStorages(orderedProducts);
        var storedProducts = result.Item1;
        
        var orderStorages = storedProducts.Select(product => new DecreaseQuantity (
            product.ProductId,
            product.StorageId,
            product.Quantity
        )).ToList();

        return orderStorages;
    }
    
    private async Task<(List<StoredProduct>, IEnumerable<StoragePoint>)> GetOrderStorages(List<ProductQuantity> orderedProducts)
    {
        var storedProducts = (await storedProductRepository.GetProductsStorages(orderedProducts)).ToList();

        var storageIds = storedProducts.Select(product => product.StorageId);
        
        var storagePoints = await storagePointRepository.GetStoragePoints(storageIds);
        
        return (storedProducts, storagePoints);
    }

    /// <summary>
    /// Возвращает примерное время доставки заказа
    /// </summary>
    /// <param name="farthestStorageDistance">
    /// Евклидово расстояние до самого далёкого склада из заказа
    /// </param>
    /// <returns>
    /// DateTime = DateTime.Now + время доставки от склада в пункт выдачи
    /// </returns>
    /// <remarks>
    /// <b>Константы расчета:</b>
    /// <code>
    /// scale = 100           // масштаб в км/координатам
    /// shiftDuration = 12      // длительность смены водителя
    /// averageSpeed = 70        // средняя скорость в км/ч
    /// </code>
    /// </remarks>
    private static DateTime CalculateDeliveryTime(double farthestStorageDistance)
    {
        const double scale = 100;
        const double shiftDuration = 12;
        const double averageSpeed = 70;

        var travelTime = farthestStorageDistance * scale / (averageSpeed * shiftDuration);
        var deliveryTime = DateTime.Now.AddHours(travelTime);
        return deliveryTime;
    }

    public async Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts)
    {
        var storedProducts = await storedProductRepository.GetByOrderedProducts(orderedProducts);

        if (storedProducts.Any(product => product.Quantity < 0))
        {
            return Result.Fail(AppError.UnprocessableContent(NotEnoughProductExceptionMessage));
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
        IEnumerable<StoragePoint> storagePoints, 
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
}