using Dapper;
using Npgsql;
using NUnit.Framework;
using Testcontainers.PostgreSql;

namespace ProductService.Tests.IntegrationTests.LocalDb;

[SetUpFixture]
public class PostgresFixture
{
    public static PostgreSqlContainer Container { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        Container = new PostgreSqlBuilder("postgres:16").Build();
        await Container.StartAsync();

        await using var connection = new NpgsqlConnection(Container.GetConnectionString());
        await connection.OpenAsync();

        await connection.ExecuteAsync(@"
        CREATE TABLE IF NOT EXISTS products (
            id        UUID         NOT NULL,
            name      VARCHAR(255) NOT NULL,
            type      INTEGER      NOT NULL,
            photo_id  UUID,

            CONSTRAINT pk_products PRIMARY KEY (id)
        );

        CREATE TABLE IF NOT EXISTS prices (
            id          UUID             NOT NULL,
            product_id  UUID             NOT NULL,
            date        TIMESTAMPTZ      NOT NULL,
            cost        DOUBLE PRECISION NOT NULL,
            discount    DECIMAL          NOT NULL,

            CONSTRAINT pk_prices PRIMARY KEY (id),
            CONSTRAINT fk_prices_products
                FOREIGN KEY (product_id)
                REFERENCES products(id)
                ON DELETE CASCADE
        );
      ");
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Container.DisposeAsync();
    }
}