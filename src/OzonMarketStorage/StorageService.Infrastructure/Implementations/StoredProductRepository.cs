using Dapper;
using StorageService.Application.Interfaces;
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

    public async Task<StoredProduct> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        var sql = @"SELECT 
                        productId AS Id, 
                        storageId AS StorageId,
                        quantity AS Quantity
                    FROM storedProducts
                    WHERE productId = @productId";
        
        var storedProduct = await connection.QueryFirstOrDefaultAsync<StoredProduct>(sql, new { id });
        return storedProduct;
    }

    public async Task Update(StoredProduct storedProduct)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "UPDATE storedProducts SET storageId = @storageId, quantity = @quantity" +
                  "WHERE productId = @productId";
        
        await connection.ExecuteAsync(sql, new
        {
            storageId =  storedProduct.StorageId,
            quantity = storedProduct.Quantity,
            productId = storedProduct.ProductId
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM storedProducts WHERE productId = @productId";

        await connection.ExecuteAsync(sql, new { id });
    }
}