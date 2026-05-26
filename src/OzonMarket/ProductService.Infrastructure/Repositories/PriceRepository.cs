using Dapper;
using ProductService.Domain;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Helpers;
using ProductService.Infrastructure.Mappers;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Repositories;

public class PriceRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPriceRepository
{
    public async Task Add(Price price)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO prices (id, data, cost, discount) " +
            "VALUES (@id, @data, @cost, @discount)";

        await connection.ExecuteAsync(sql, param: new
        {
            id = price.Id,
            data = price.Data,
            cost = price.Cost,
            discount = price.Discount
        });
    }

    public async Task<Price?> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        await using var command = connection.CreateCommand();

        var sql = "select id, data, cost, discount from prices where id = @id";

        var dao = await connection.QueryFirstOrDefaultAsync<PriceDao>(sql, new { id });
        return dao.ToDomain();
    }
}