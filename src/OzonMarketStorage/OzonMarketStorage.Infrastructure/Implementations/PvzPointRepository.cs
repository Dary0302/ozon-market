using Dapper;
using OzonMarketStorage.Application.Interfaces;
using OzonMarketStorage.Domain;
using OzonMarketStorage.Infrastructure.Helpers;

namespace OzonMarketStorage.Infrastructure.Implementations;

public class PvzPointRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPvzPointRepository
{
    public async Task Add(PvzPoint pvzPoint)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO pvzPoints (pointId, pvzId, longitude, latitude)" +
                  "VALUES (@pointId, @pvzId, @longitude, @latitude)";
        
        await connection.ExecuteAsync(sql, new
        {
            pointId = pvzPoint.PointId,
            pvzId = pvzPoint.PvzId,
            longitude = pvzPoint.Longitude,
            latitude = pvzPoint.Latitude
        });
    }

    public async Task<PvzPoint> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        var sql = @"SELECT 
                        pointId AS PointId, 
                        pvzId AS PvzId,
                        longitude AS Longitude,
                        latitude AS Latitude
                    FROM pvzPoints 
                    WHERE id = @id";
        
        var pvzPoint = await connection.QueryFirstOrDefaultAsync<PvzPoint>(sql, new { id });
        return pvzPoint;
    }

    public async Task Update(PvzPoint pvzPoint)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "UPDATE pvzPoints SET pvzId = @pvzId, longitude = @longitude, latitude = @latitude" +
                  "WHERE pointId = @pointId";
        
        await connection.ExecuteAsync(sql, new
        {
            pvzId =  pvzPoint.PvzId,
            longitude = pvzPoint.Longitude,
            latitude = pvzPoint.Latitude,
            pointId = pvzPoint.PointId
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM pvzPoints WHERE pointId = @pointId";

        await connection.ExecuteAsync(sql, new { id });
    }
}