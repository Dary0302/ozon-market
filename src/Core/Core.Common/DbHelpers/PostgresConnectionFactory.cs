using Core.Common.DbHelpers.Interfaces;
using Npgsql;

namespace Core.Common.DbHelpers;

public class PostgresConnectionFactory(string connectionString) : IPostgresConnectionFactory
{
    public NpgsqlConnection GetConnection() => new(connectionString);
}