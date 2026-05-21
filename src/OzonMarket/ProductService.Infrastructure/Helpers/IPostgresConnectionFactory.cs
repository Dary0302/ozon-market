using Npgsql;

namespace ProductService.Infrastructure.Helpers;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}