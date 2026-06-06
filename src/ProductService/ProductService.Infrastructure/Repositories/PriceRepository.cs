using Core.Common.DbHelpers.Interfaces;
using Dapper;
using ProductService.Domain;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Mappers;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Repositories;

public class PriceRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPriceRepository
{
    public async Task SetPrice(Price price, CancellationToken cancellationToken)
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

        var command = new CommandDefinition(
            sql,
            new
            {
                price.Id,
                price.ProductId,
                price.Date,
                price.Cost,
                price.Discount
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task<Price?> GetPrice(
        Guid productId,
        CancellationToken cancellationToken)
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

        var command = new CommandDefinition(
            sql,
            new { productId },
            cancellationToken: cancellationToken);

        var dao = await connection.QueryFirstOrDefaultAsync<PriceDao>(command);

        return dao?.ToDomain();
    }

    public async Task<IEnumerable<Price?>> GetPrices(
        List<Guid> productIds,
        CancellationToken cancellationToken)
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

        var command = new CommandDefinition(
            sql,
            new { productIds },
            cancellationToken: cancellationToken);

        var daos = await connection.QueryAsync<PriceDao>(command);

        return daos.Select(dao => dao.ToDomain());
    }
}