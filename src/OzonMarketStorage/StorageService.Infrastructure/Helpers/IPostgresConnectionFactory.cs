using Npgsql;

namespace StorageService.Infrastructure.Helpers;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}