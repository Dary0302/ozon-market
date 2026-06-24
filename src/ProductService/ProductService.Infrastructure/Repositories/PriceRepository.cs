using System.Text;
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

        const string sql = """
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
                Id = Guid.NewGuid(),
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
        DateTime? priceDate = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT
                      id,
                      product_id,
                      "date",
                      cost,
                      discount
                  FROM prices
                  WHERE product_id = @productId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("productId", productId);

        if (priceDate.HasValue)
        {
            sql += """

                   AND "date" <= @priceDate
                   """;

            parameters.Add("priceDate", priceDate.Value);
        }

        sql += """

               ORDER BY "date" DESC
               LIMIT 1
               """;

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        var dao = await connection.QueryFirstOrDefaultAsync<PriceDao>(command);
        return dao?.ToDomain();
    }
    
    public async Task<IEnumerable<Price>> GetPrices(
        List<Guid> productIds,
        DateTime? priceDate = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = new StringBuilder("""
                                    SELECT DISTINCT ON (product_id)
                                           id,
                                           product_id,
                                           "date",
                                           cost,
                                           discount
                                    FROM prices
                                    WHERE product_id = ANY(@productIds)
                                    """);

        var parameters = new DynamicParameters();
        parameters.Add("productIds", productIds);

        if (priceDate.HasValue)
        {
            sql.AppendLine("""
                           AND "date" <= @priceDate
                           """);

            parameters.Add("priceDate", priceDate.Value);
        }

        sql.AppendLine("""
                       ORDER BY product_id, "date" DESC
                       """);

        var command = new CommandDefinition(
            sql.ToString(),
            parameters,
            cancellationToken: cancellationToken);

        var daos = await connection.QueryAsync<PriceDao>(command);

        return daos.Select(dao => dao.ToDomain()!);
    }
}