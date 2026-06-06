using System.Text;
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
                  INSERT INTO products (id, name, description, type, photoId)
                  VALUES (@id, @name, @description, @type, @photoId)
                  """;

        await connection.ExecuteAsync(sql,
            new
            { 
                id = product.Id, 
                name = product.Name, 
                description = product.Description, 
                type = product.Type,
                photoId = product.PhotoId
            });
    }

    public async Task<Product?> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  SELECT id, name, description, type, photoId
                  FROM products
                  WHERE id = @id
                  """;

        var dao = await connection.QueryFirstOrDefaultAsync<ProductDao>(sql, new { id });
        return dao.ToDomain();
    }
    
    public async Task<IReadOnlyCollection<Product?>> GetProductsByFilter(ProductFilter filter)
{
    await using var connection = postgresConnectionFactory.GetConnection();

    var sql = new StringBuilder(
        """
        SELECT
            product.id,
            product.name,
            product.description,
            product.type,
            product.photo_id
        FROM products product
        INNER JOIN prices price
            ON price.product_id = product.id
        WHERE 1 = 1
        """);

    var parameters = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(filter.Name))
    {
        sql.AppendLine("""
            AND LOWER(product.name) LIKE LOWER(@name)
            """);

        parameters.Add("name", $"%{filter.Name}%");
    }

    if (filter.Types?.Any() == true)
    {
        sql.AppendLine("""
            AND product.type = ANY(@types)
            """);

        parameters.Add("types", filter.Types.ToArray());
    }

    if (filter.MinPrice.HasValue)
    {
        sql.AppendLine("""
            AND price.cost >= @minPrice
            """);

        parameters.Add("minPrice", filter.MinPrice.Value);
    }

    if (filter.MaxPrice.HasValue)
    {
        sql.AppendLine("""
            AND price.cost <= @maxPrice
            """);

        parameters.Add("maxPrice", filter.MaxPrice.Value);
    }

    if (filter.HasDiscount.HasValue)
    {
        if (filter.HasDiscount.Value)
        {
            sql.AppendLine("""
                AND price.discount > 0
                """);
        }
        else
        {
            sql.AppendLine("""
                AND (price.discount IS NULL OR price.discount = 0)
                """);
        }
    }

    if (filter.MinDiscount.HasValue)
    {
        sql.AppendLine("""
            AND price.discount >= @minDiscount
            """);

        parameters.Add("minDiscount", filter.MinDiscount.Value);
    }

    if (filter.MaxDiscount.HasValue)
    {
        sql.AppendLine("""
            AND price.discount <= @maxDiscount
            """);

        parameters.Add("maxDiscount", filter.MaxDiscount.Value);
    }

    var offset = (filter.Page - 1) * filter.PageSize;

    sql.AppendLine("""
                   ORDER BY product.name
                   LIMIT @pageSize
                   OFFSET @offset
                   """);

    parameters.Add("pageSize", filter.PageSize);
    parameters.Add("offset", offset);
    
    var daos = await connection.QueryAsync<ProductDao>(
        sql.ToString(),
        parameters);

    return daos
        .Select(x => x.ToDomain())
        .ToList();
}
    
    public async Task Update(Guid id, Product product)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  UPDATE products
                  SET
                      name = @name,
                      description = @description,
                      type = @type,
                      photoId = @photoId
                  WHERE id = @Id
                  """;

        await connection.ExecuteAsync(sql, new
        {
            id = id,
            name = product.Name,
            description = product.Description,
            type = product.Type,
            photoId = product.PhotoId
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