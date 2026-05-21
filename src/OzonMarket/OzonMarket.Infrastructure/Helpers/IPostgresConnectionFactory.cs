using Npgsql;

namespace OzonMarket.Infrastructure.Helpers;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}