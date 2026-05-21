using Npgsql;

namespace OzonMarket.Infrastructure.Helpers;

public class PostgresConnectionFactory(string connectionString) : IPostgresConnectionFactory
{
    public NpgsqlConnection GetConnection() => new(connectionString);
}