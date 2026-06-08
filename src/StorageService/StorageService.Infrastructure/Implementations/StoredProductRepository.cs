using Dapper;
using StorageService.Domain;
using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class StoredProductRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStoredProductRepository
{
    public async Task Add(StoredProduct storedProduct, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO stored_products (product_id, storage_id, quantity)
                  VALUES (@productId, @storageId, @quantity)
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                productId = storedProduct.ProductId,
                storageId = storedProduct.StorageId,
                quantity = storedProduct.Quantity
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }
    
    public async Task<IEnumerable<StoredProduct>> GetByOrderedProducts(List<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
    
        var sql = """
                    SELECT 
                        sp.product_id AS ProductId, 
                        sp.storage_id AS StorageId,
                        sp.quantity - req.quantity AS Quantity
                    FROM stored_products sp
                    JOIN UNNEST(@ProductIds::uuid[], @StorageIds::uuid[], @Quantities::int[]) 
                        AS req(product_id, storage_id, quantity)
                        ON sp.product_id = req.product_id 
                        AND sp.storage_id = req.storage_id
                  """;
    
        var productIds = orderedProducts.Select(x => x.ProductId).ToArray();
        var storageIds = orderedProducts.Select(x => x.StorageId).ToArray();
        var quantities = orderedProducts.Select(x => x.Quantity).ToArray();

        var command = new CommandDefinition(
            sql,
            new
            {
                ProductIds = productIds,
                StorageIds = storageIds,
                Quantities = quantities
            },
            cancellationToken: cancellationToken);
    
        var daos =  await connection.QueryAsync<StoredProductDao>(command);

        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }

    public async Task<IEnumerable<ProductQuantity>> GetAllInStock(CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT product_id, SUM(quantity) AS quantity
                  FROM stored_products
                  GROUP BY product_id
                  HAVING SUM(quantity) > 0
                  """;

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);
        
        var daos = await connection.QueryAsync<ProductQuantityDao>(command);

        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }
    
    public async Task<IEnumerable<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
    {
        if (productIds is null || !productIds.Any())
        {
            return Enumerable.Empty<ProductQuantity>();
        }
    
        await using var connection = postgresConnectionFactory.GetConnection();
    
        var productIdsArray = productIds.ToArray();

        var sql = """
                  SELECT 
                      product_id AS ProductId, 
                      SUM(quantity) AS Quantity
                  FROM stored_products
                  WHERE product_id = ANY(@ProductIds)
                  GROUP BY product_id
                  """;

        var command = new CommandDefinition(
            sql,
            new { ProductIds = productIdsArray },
            cancellationToken: cancellationToken);
    
        var daos = await connection.QueryAsync<ProductQuantityDao>(command);

        var storedProducts = daos.Select(dao => dao.ToDomain());
    
        return storedProducts;
    }
    
    public async Task<IEnumerable<StoredProduct>> GetProductsStorages(List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        if (orderedProducts is null || !orderedProducts.Any())
        {
            return Enumerable.Empty<StoredProduct>();
        }
        
        var productIds = orderedProducts.Select(product => product.ProductId).ToArray();;
        
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT product_id, storage_id, quantity
                  FROM stored_products
                  WHERE product_id = ANY(@productIds)
                  """;

        var command = new CommandDefinition(
            sql,
            new { productIds },
            cancellationToken: cancellationToken);
        
        var daos = await connection.QueryAsync<StoredProductDao>(command);

        daos = GetSuitableStorages(daos, orderedProducts);
        
        var storedProducts = daos.Select(dao => dao.ToDomain());
        
        return storedProducts;
    }

    public async Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                    UPDATE stored_products sp
                    SET quantity = sp.quantity - req.quantity
                    FROM UNNEST(@ProductIds::uuid[], @Quantities::int[]) 
                        AS req(product_id, quantity)
                    WHERE sp.product_id = req.product_id 
                      AND sp.quantity >= req.quantity
                  """;
        
        var productIds = orderedProducts.Select(x => x.ProductId).ToArray();
        var quantities = orderedProducts.Select(x => x.Quantity).ToArray();

        var command = new CommandDefinition(
            sql,
            new
            {
                ProductIds = productIds,
                Quantities = quantities
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
    
    public async Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                    UPDATE stored_products sp
                    SET quantity = sp.quantity + req.quantity
                    FROM UNNEST(@ProductIds::uuid[], @Quantities::int[]) 
                        AS req(product_id, quantity)
                    WHERE sp.product_id = req.product_id
                  """;
        
        var productIds = arrivedProducts.Select(x => x.ProductId).ToArray();
        var quantities = arrivedProducts.Select(x => x.Quantity).ToArray();

        var command = new CommandDefinition(
            sql,
            new
            {
                ProductIds = productIds,
                Quantities = quantities
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  DELETE FROM stored_products WHERE product_id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
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
            .Select(product => new StoredProductDao
            {
                ProductId = product.ProductId,
                StorageId = product.StorageId,
                Quantity = product.StoredQuantity
            });
        
        return suitableStorages;
    }
}