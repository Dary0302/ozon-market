using Dapper;
using OzonMarket.Domain;
using OzonMarket.Domain.Interfaces;
using OzonMarket.Infrastructure.Helpers;

namespace OzonMarket.Infrastructure.Repositories;

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

    public async Task<Product> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        await using var command = connection.CreateCommand();

        var sql = "select id, name, description, type from products where id = @id";

        var dao = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { id });
        return dao;
    }
}