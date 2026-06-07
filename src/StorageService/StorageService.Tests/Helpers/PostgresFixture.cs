using Xunit;
using Dapper;
using Npgsql;
using Testcontainers.PostgreSql;

namespace StorageService.Tests.Helpers;

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
            CREATE TABLE IF NOT EXISTS storages (
                id            UUID           NOT NULL,
                address       VARCHAR(255)   NOT NULL,
                point_id      UUID           NOT NULL,

                CONSTRAINT pk_storages PRIMARY KEY (id)
            );

            CREATE TABLE IF NOT EXISTS pvz (
                id            UUID           NOT NULL,
                address       VARCHAR(255)   NOT NULL,
                point_id      UUID           NOT NULL,

                CONSTRAINT pk_pvz PRIMARY KEY (id)
            );

            CREATE TABLE IF NOT EXISTS storage_points (
                id            UUID           NOT NULL,
                storage_id    UUID           NOT NULL,
                longitude     DOUBLE PRECISION NOT NULL,
                latitude      DOUBLE PRECISION NOT NULL,

                CONSTRAINT pk_storage_points PRIMARY KEY (id),
                CONSTRAINT fk_storage_points_storages FOREIGN KEY (storage_id) 
                    REFERENCES storages(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS pvz_points (
                id            UUID           NOT NULL,
                pvz_id        UUID           NOT NULL,
                longitude     DOUBLE PRECISION NOT NULL,
                latitude      DOUBLE PRECISION NOT NULL,

                CONSTRAINT pk_pvz_points PRIMARY KEY (id),
                CONSTRAINT fk_pvz_points_pvz FOREIGN KEY (pvz_id) 
                    REFERENCES pvz(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS stored_products (
                product_id    UUID           NOT NULL,
                storage_id    UUID           NOT NULL,
                quantity      INT            NOT NULL,

                CONSTRAINT pk_stored_products PRIMARY KEY (product_id, storage_id),
                CONSTRAINT fk_stored_products_storages FOREIGN KEY (storage_id) 
                    REFERENCES storages(id) ON DELETE CASCADE
            );");
    }

    public async Task DisposeAsync() => await Container.DisposeAsync();
}