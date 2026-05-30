using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using StorageService.Application.Interfaces.Repositories;

namespace StorageService.Infrastructure.Implementations;

public class StorageRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStorageRepository
{
    public async Task Add(Storage storage)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO storages (id, address, pointId)" +
                  "VALUES (@id, @address, @pointId)";
        
        await connection.ExecuteAsync(sql, new
        {
            id = storage.Id,
            address = storage.Address,
            pointId = storage.PointId
        });
    }

    public async Task<Storage> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        var sql = @"SELECT 
                        id AS Id, 
                        address AS Address,
                        pointId AS PointId
                    FROM storages 
                    WHERE id = @id";
        
        var storage = await connection.QueryFirstOrDefaultAsync<Storage>(sql, new { id });
        return storage;
    }

    public async Task Update(Storage storage)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "UPDATE storages SET address = @address, pointId = @pointId WHERE id = @id";
        
        await connection.ExecuteAsync(sql, new
        {
            address = storage.Address,
            pointId = storage.PointId,
            id = storage.Id
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM storages WHERE id = @id";

        await connection.ExecuteAsync(sql, new { id });
    }
}