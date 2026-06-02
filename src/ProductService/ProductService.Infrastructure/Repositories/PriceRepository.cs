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
                  INSERT INTO prices
                  (
                      id,
                      product_id,
                      date,
                      cost,
                      discount
                  )
                  VALUES
                  (
                      @Id,
                      @ProductId,
                      @Date,
                      @Cost,
                      @Discount
                  )
                  """;

        await connection.ExecuteAsync(sql, new
        {
            id = price.Id, 
            productId = price.ProductId, 
            date = price.Date,
            cost = price.Cost,
            discount = price.Discount
        });
    }

    public async Task<Price?> GetPrice(Guid productId)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT
                      id,
                      product_id,
                      date,
                      cost,
                      discount
                  FROM prices
                  WHERE product_id = @productId
                  ORDER BY date DESC
                  LIMIT 1
                  """;

        var dao = await connection.QueryFirstOrDefaultAsync<PriceDao>(
            sql,
            new { productId });

        return dao.ToDomain();
    }

    public async Task<IEnumerable<Price?>> GetPrices(List<Guid> productIds)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT DISTINCT ON (product_id)
                         id,
                         product_id,
                         date,
                         cost,
                         discount
                  FROM prices
                  WHERE product_id = ANY(@productIds)
                  ORDER BY product_id, date DESC
                  """;

        var daos = await connection.QueryAsync<PriceDao>(
            sql,
            new { productIds });

        return daos
            .Select(x => x.ToDomain());
    }
}