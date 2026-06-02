using Dapper;
using StorageService.Domain;
using Core.Common.DbHelpers;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class StoredProductRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStoredProductRepository
{
    public async Task Add(StoredProduct storedProduct)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO storedProducts (productId, storageId, quantity)" +
                  "VALUES (@productId, @storageId, @quantity)";
        
        await connection.ExecuteAsync(sql, new
        {
            productId = storedProduct.ProductId,
            storageId = storedProduct.StorageId,
            quantity = storedProduct.Quantity
        });
    }
    
    public async Task<IEnumerable<StoredProduct>> GetByOrderedProducts(List<DecreaseQuantity> orderedProducts)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
    
        var sql = @"
                    SELECT 
                        sp.productId AS Id, 
                        sp.storageId AS StorageId,
                        sp.quantity - req.quantity AS Quantity
                    FROM storedProducts sp
                    JOIN UNNEST(@ProductIds::uuid[], @StorageIds::uuid[], @Quantities::int[]) 
                        AS req(productId, storageId, quantity)
                        ON sp.productId = req.productId 
                        AND sp.storageId = req.storageId";
    
        var productIds = orderedProducts.Select(x => x.ProductId).ToArray();
        var storageIds = orderedProducts.Select(x => x.StorageId).ToArray();
        var quantities = orderedProducts.Select(x => x.Quantity).ToArray();
    
        var daos =  await connection.QueryAsync<StoredProductDao>(sql, new
        {
            ProductIds = productIds,
            StorageIds = storageIds,
            Quantities = quantities
        });

        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }

    public async Task<IEnumerable<ProductQuantity>> GetAllInStock()
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, SUM(quantity) AS quantity
                    FROM storedProducts
                    GROUP BY productId
                    HAVING totalQuantity > 0";
        
        var daos = await connection.QueryAsync<ProductQuantityDao>(sql);

        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }
    
    public async Task<IEnumerable<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, SUM(quantity) AS quantity
                    FROM storedProducts
                    WHERE productId IN @productIds 
                    GROUP BY productId";
        
        var daos = await connection.QueryAsync<ProductQuantityDao>(sql, new { productIds });

        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }
    
    public async Task<IEnumerable<StoredProduct>> GetProductsStorages(List<ProductQuantity> orderedProducts)
    {
        var productIds = orderedProducts.Select(product => product.ProductId);
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, storageId, quantity
                    FROM storedProducts
                    WHERE productId IN @productIds";
        
        var daos = await connection.QueryAsync<StoredProductDao>(sql, new { productIds });

        daos = GetSuitableStorages(daos, orderedProducts);
        
        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }

    public async Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"
                    UPDATE storedProducts sp
                    SET Quantity = sp.Quantity - req.quantity
                    FROM UNNEST(@ProductIds::uuid[], @Quantities::int[]) 
                        AS req(productId, quantity)
                    WHERE sp.Id = req.productId 
                      AND sp.Quantity >= req.quantity";
        
        var productIds = orderedProducts.Select(x => x.ProductId).ToArray();
        var quantities = orderedProducts.Select(x => x.Quantity).ToArray();

        await connection.ExecuteAsync(sql, new
        {
            ProductIds = productIds,
            Quantities = quantities
        });
    }
    
    public async Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"
                    UPDATE storedProducts sp
                    SET Quantity = sp.Quantity + req.quantity
                    FROM UNNEST(@ProductIds::uuid[], @Quantities::int[]) 
                        AS req(productId, quantity)
                    WHERE sp.Id = req.productId";
        
        var productIds = arrivedProducts.Select(x => x.ProductId).ToArray();
        var quantities = arrivedProducts.Select(x => x.Quantity).ToArray();

        await connection.ExecuteAsync(sql, new
        {
            ProductIds = productIds,
            Quantities = quantities
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM storedProducts WHERE productId = @productId";

        await connection.ExecuteAsync(sql, new { id });
    }
    
    /// <summary>
    /// Возвращает список из складов, в которых достаточно продуктов для заказа
    /// </summary>
    private static IEnumerable<StoredProductDao> GetSuitableStorages(IEnumerable<StoredProductDao> storedProducts, List<ProductQuantity> orderedProducts)
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
            .Select(product => new StoredProductDao(product.ProductId, product.StorageId, product.StoredQuantity));
        
        return suitableStorages;
    }
}