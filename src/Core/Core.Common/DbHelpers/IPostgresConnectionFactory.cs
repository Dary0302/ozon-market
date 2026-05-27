using Npgsql;

namespace Core.Common.DbHelpers;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}