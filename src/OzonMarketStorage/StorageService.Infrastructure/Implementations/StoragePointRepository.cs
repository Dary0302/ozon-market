using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using StorageService.Application.Interfaces.Repositories;

namespace StorageService.Infrastructure.Implementations;

public class StoragePointRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStoragePointRepository
{
    public async Task Add(StoragePoint storagePoint)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO storagePoints (pointId, storageId, longitude, latitude)" +
                  "VALUES (@pointId, @storageId, @longitude, @latitude)";
        
        await connection.ExecuteAsync(sql, new
        {
            pointId = storagePoint.Id,
            storageId = storagePoint.StorageId,
            longitude = storagePoint.Longitude,
            latitude = storagePoint.Latitude
        });
    }

    public async Task<StoragePoint?> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = @"SELECT 
                        pointId AS Id, 
                        storageId AS StorageId,
                        longitude AS Longitude,
                        latitude AS Latitude
                    FROM storagePoints 
                    WHERE id = @id";
        
        var storagePoint = await connection.QueryFirstOrDefaultAsync<StoragePoint>(sql, new { id });
        return storagePoint;
    }

    public async Task<IEnumerable<StoragePoint>> GetStoragePoints(IEnumerable<Guid> storageIds)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = @"SELECT 
                        pointId AS Id, 
                        storageId AS StorageId,
                        longitude AS Longitude,
                        latitude AS Latitude
                    FROM storagePoints 
                    WHERE id IN @storageIds";
        
        var storagePoints = await connection.QueryAsync<StoragePoint>(sql, new { storageIds });
        return storagePoints;
    }

    public async Task Update(StoragePoint storagePoint)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "UPDATE storagePoints SET storageId = @storageId, longitude = @longitude, latitude = @latitude" +
                  "WHERE pointId = @pointId";
        
        await connection.ExecuteAsync(sql, new
        {
            storageId =  storagePoint.StorageId,
            longitude = storagePoint.Longitude,
            latitude = storagePoint.Latitude,
            pointId = storagePoint.Id
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM storagePoints WHERE pointId = @pointId";

        await connection.ExecuteAsync(sql, new { id });
    }
}