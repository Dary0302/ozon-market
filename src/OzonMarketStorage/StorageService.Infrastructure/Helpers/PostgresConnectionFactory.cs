using Npgsql;

namespace StorageService.Infrastructure.Helpers;

public class PostgresConnectionFactory(string connectionString) : IPostgresConnectionFactory
{
    public NpgsqlConnection GetConnection() => new(connectionString);
}