using Core.Common.DbHelpers.Interfaces;
using Dapper;
using ProductService.Domain;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Mappers;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Repositories;

public class ProductRepository(IPostgresConnectionFactory postgresConnectionFactory) : IProductRepository
{
    public async Task Add(Product product)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO products (id, name, description, type)
                  VALUES (@id, @name, @description, @type)
                  """;

        await connection.ExecuteAsync(sql,
            new
            { 
                id = product.Id, 
                name = product.Name, 
                description = product.Description, 
                type = product.Type 
            });
    }

    public async Task<Product?> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT id, name, description, type
                  FROM products
                  WHERE id = @id
                  """;

        var dao = await connection.QueryFirstOrDefaultAsync<ProductDao>(sql, new { id });
        return dao.ToDomain();
    }
    
    public async Task Update(Guid id, Product product)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  UPDATE products
                  SET
                      name = @Name,
                      description = @Description,
                      type = @Type
                  WHERE id = @Id
                  """;

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            product.Name,
            product.Description,
            product.Type
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  DELETE FROM products
                  WHERE id = @id
                  """;

        await connection.ExecuteAsync(sql, new { id });
    }
}