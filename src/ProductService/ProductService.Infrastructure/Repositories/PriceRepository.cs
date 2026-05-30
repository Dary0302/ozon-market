using Core.Common.DbHelpers.Interfaces;
using Dapper;
using ProductService.Domain;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Mappers;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Repositories;

public class PriceRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPriceRepository
{
    public async Task SetPrice(Price price)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                   INSERT INTO prices (id, data, cost, discount)
                   VALUES (@id, @data, @cost, @discount)
                   """;
        await connection.ExecuteAsync(sql, param: new
        {
            id = price.Id,
            data = price.Date,
            cost = price.Cost,
            discount = price.Discount
        });
    }

    public async Task<Price?> GetPrice(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT id, data, cost, discount
                  FROM prices
                  WHERE id = @id
                  """;

        var dao = await connection.QueryFirstOrDefaultAsync<PriceDao>(sql, new { id });
        return dao.ToDomain();
    }
}