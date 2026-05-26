using Core.Common.DbHelpers;
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

        var sql = "INSERT INTO products (id, name, description, type) " +
            "VALUES (@id, @name, @description, @type)";

        await connection.ExecuteAsync(sql, param: new
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
        await using var command = connection.CreateCommand();

        var sql = "select id, name, description, type from products where id = @id";

        var dao = await connection.QueryFirstOrDefaultAsync<ProductDao>(sql, new { id });
        return dao.ToDomain();
    }
}