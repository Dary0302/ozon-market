using Npgsql;

namespace OzonMarketStorage.Infrastructure.Helpers;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}