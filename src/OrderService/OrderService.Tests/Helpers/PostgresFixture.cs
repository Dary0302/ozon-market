using System.Data;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace OrderService.Tests.Helpers;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public PostgresFixture()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    
    public async Task InitializeAsync()
    {
        await Container.StartAsync();
        
        await using var connection = new NpgsqlConnection(Container.GetConnectionString());
        await connection.OpenAsync();
        await connection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS orders (
                id            UUID           NOT NULL,
                pvz_id        UUID           NOT NULL,
                created_on    TIMESTAMP      NOT NULL,
                delivery_date TIMESTAMP      NOT NULL,
                status        INT            NOT NULL,
                amount        DECIMAL(12,2)  NOT NULL,

                CONSTRAINT pk_orders PRIMARY KEY (id)
            );

            CREATE TABLE IF NOT EXISTS order_items (
                order_id    UUID  NOT NULL,
                product_id  UUID  NOT NULL,
                quantity    INT   NOT NULL,

                CONSTRAINT pk_order_items PRIMARY KEY (order_id, product_id),
                CONSTRAINT fk_order_items_orders FOREIGN KEY (order_id) 
                    REFERENCES orders(id) ON DELETE CASCADE
            );");
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();
}