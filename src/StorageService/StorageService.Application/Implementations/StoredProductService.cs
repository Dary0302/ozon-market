using Core.Common.Errors;
using FluentResults;
using StorageService.Application.Dto;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Implementations;

public class StoredProductService(
    IStoredProductRepository storedProductRepository,
    IStoragePointRepository storagePointRepository,
    IPvzPointRepository pvzPointRepository,
    IPvzRepository pvzRepository,
    IStorageRepository storageRepository) : IStoredProductService
{
    private const string NotEnoughProductExceptionMessage = "Не хватает товара на складе";
    
    public async Task<Result> AddStoredProduct(StoredProduct storedProduct, CancellationToken cancellationToken)
    {
        var existingProduct = await storedProductRepository.GetProductsQuantity(
            new[] { storedProduct.ProductId }, 
            cancellationToken);
    
        if (existingProduct.Any())
        {
            return Result.Fail(AppError.UnprocessableContent("Продукт уже существует на этом складе"));
        }
    
        await storedProductRepository.Add(storedProduct, cancellationToken);
    
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<ProductQuantity>>> GetStoredProductsInStock(CancellationToken cancellationToken)
    {
        var storedProducts = await storedProductRepository.GetAllInStock(cancellationToken);

        return Result.Ok(storedProducts);
    }

    public async Task<Result<IEnumerable<StockCheckResult>>> CheckStock(List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var productIds = orderedProducts.Select(product => product.ProductId);
        
        var storedProducts = (await storedProductRepository.GetProductsQuantity(productIds, cancellationToken)).ToList();

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
    
    public async Task<Result<DateTime>> GetDeliveryDate(Guid pvzId, List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var pvz = await pvzRepository.Get(pvzId, cancellationToken);

        if (pvz is null)
        {
            return Result.Fail(AppError.NotFound("Пвз не найден"));
        }
        
        var pvzPoint =  await pvzPointRepository.Get(pvz.PointId, cancellationToken);
        
        if (pvzPoint is null)
        {
            return Result.Fail(AppError.NotFound("Точка ПВЗ не найдена"));
        }
        
        var storedProducts = (await GetProductsStorages(orderedProducts, cancellationToken)).ToList();
        var storagePoints = await GetOrderStorages(storedProducts, cancellationToken);
        
        var chosenStorages = ChooseOrderStorages(storedProducts, storagePoints, pvzPoint);
        
        if (chosenStorages is null || chosenStorages.Count == 0)
        {
            return Result.Fail(AppError.UnprocessableContent("Складов с нужным количеством товара не обнаружено"));
        }

        var farthestStorageDistance = chosenStorages.Max(product => product.Distance);

        var deliveryTime = CalculateDeliveryTime(farthestStorageDistance);

        return Result.Ok(deliveryTime);
    }

    public async Task<Result<List<DecreaseQuantity>>> GetOrderStoragesRecords(Guid pvzId, List<ProductQuantity> orderedProducts, 
        CancellationToken cancellationToken)
    {
        var pvz = await pvzRepository.Get(pvzId, cancellationToken);

        if (pvz is null)
        {
            return Result.Fail(AppError.NotFound("Пвз не найден"));
        }
        
        var storedProducts = (await GetProductsStorages(orderedProducts, cancellationToken)).ToList();
        
        var orderStorages = storedProducts.Select(product => new DecreaseQuantity (
            product.ProductId,
            product.StorageId,
            product.Quantity
        )).ToList();

        return orderStorages;
    }

    public async Task<Result> DecreaseStoredProductQuantity(List<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var storedProducts = await storedProductRepository.GetByOrderedProducts(orderedProducts, cancellationToken);

        if (storedProducts.Any(product => product.Quantity < 0))
        {
            return Result.Fail(AppError.UnprocessableContent(NotEnoughProductExceptionMessage));
        }

        await storedProductRepository.DecreaseCount(orderedProducts, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> IncreaseStoredProductQuantity(List<IncreaseQuantity> arrivedProducts, CancellationToken cancellationToken)
    {
        await storedProductRepository.IncreaseCount(arrivedProducts, cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result> ReturnProducts(IEnumerable<ProductQuantity> returnedProducts, CancellationToken cancellationToken)
    {
        var storages = (await storageRepository.GetAll(cancellationToken)).ToList();
        
        var closestStorage = storages[Random.Shared.Next(storages.Count)];

        var products = returnedProducts.Select(product => new IncreaseQuantity(product.ProductId, closestStorage.Id, product.Quantity));

        await storedProductRepository.IncreaseCount(products, cancellationToken);
        
        return Result.Ok();
    }
    
    public async Task<IEnumerable<StoredProduct>> GetProductsStorages(List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var storedProducts = await storedProductRepository.GetProductsStorages(orderedProducts, cancellationToken);

        return storedProducts;
    }

    public async Task<IEnumerable<StoragePoint>> GetOrderStorages(List<StoredProduct> storedProducts, CancellationToken cancellationToken)
    {
        var storageIds = storedProducts.Select(product => product.StorageId);
        
        var storagePoints = await storagePointRepository.GetStoragePoints(storageIds, cancellationToken);

        return storagePoints;
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

    /// <summary>
    /// Возвращает коллекцию с разницей товаров на складах и заказанных товаров
    /// </summary>
    private IEnumerable<StockCheckResult> CheckStock(List<ProductQuantity> storedProducts,  List<ProductQuantity> orderedProducts)
    {
        var stockCheckResults = storedProducts.Join(orderedProducts,
            storedProduct => storedProduct.ProductId,
            orderedProduct => orderedProduct.ProductId,
            (storedProduct, orderedProduct) => new StockCheckResult {
                ProductId = storedProduct.ProductId,
                Difference = storedProduct.Quantity - orderedProduct.Quantity,
            });

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