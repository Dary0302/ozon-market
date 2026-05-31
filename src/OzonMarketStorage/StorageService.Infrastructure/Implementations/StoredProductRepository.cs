using Dapper;
using StorageService.Domain;
using Core.Common.DbHelpers;
using StorageService.Application.Interfaces.Repositories;

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
        
        var sql = @"SELECT 
                        productId AS Id, 
                        storageId AS StorageId,
                        quantity - @quantity AS Quantity
                    FROM storedProducts
                    WHERE productId = @productId
                    AND storageId = @storageId";
        
        var storedProducts = await connection.QueryAsync<StoredProduct>(sql, orderedProducts);
        return storedProducts;
    }

    public async Task<IEnumerable<ProductQuantity>> GetAllInStock()
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, SUM(quantity) AS quantity
                    FROM storedProducts
                    GROUP BY productId
                    HAVING totalQuantity > 0";
        
        var storedProducts = await connection.QueryAsync<ProductQuantity>(sql);
        
        return storedProducts;
    }
    
    public async Task<List<ProductQuantity>> GetProductsQuantity(IEnumerable<Guid> productIds)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, SUM(quantity) AS quantity
                    FROM storedProducts
                    WHERE productId IN @productIds 
                    GROUP BY productId";
        
        var storedProducts = await connection.QueryAsync<ProductQuantity>(sql, new { productIds });
        
        return storedProducts.ToList();
    }
    
    public async Task<List<StoredProduct>> GetProductsStorages(IEnumerable<Guid> productIds)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"SELECT productId, storageId, quantity
                    FROM storedProducts
                    WHERE productId IN @productIds";
        
        var storedProducts = await connection.QueryAsync<StoredProduct>(sql, new { productIds });
        
        return storedProducts.ToList();
    }

    public async Task DecreaseCount(IEnumerable<DecreaseQuantity> orderedProducts)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"UPDATE storedProducts 
            SET Quantity = Quantity - @Quantity 
            WHERE Id = @ProductId 
            AND Quantity >= @Quantity"; 

        await connection.ExecuteAsync(sql, orderedProducts);
    }
    
    public async Task IncreaseCount(IEnumerable<IncreaseQuantity> arrivedProducts)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = @"UPDATE storedProducts 
            SET Quantity = Quantity + @Quantity 
            WHERE Id = @ProductId"; 

        await connection.ExecuteAsync(sql, arrivedProducts);
    }
    

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM storedProducts WHERE productId = @productId";

        await connection.ExecuteAsync(sql, new { id });
    }
}