using Npgsql;

namespace Core.Common.DbHelpers.Interfaces;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}