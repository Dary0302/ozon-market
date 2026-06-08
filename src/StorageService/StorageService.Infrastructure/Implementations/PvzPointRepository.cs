using Dapper;
using StorageService.Domain;
using Core.Common.DbHelpers.Interfaces;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class PvzPointRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPvzPointRepository
{
    public async Task Add(PvzPoint pvzPoint, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO pvz_points (id, pvz_id, longitude, latitude)
                  VALUES (@pointId, @pvzId, @longitude, @latitude)
                  """;

        var command = new CommandDefinition(
            sql, 
            new
            {
                pointId = pvzPoint.Id,
                pvzId = pvzPoint.PvzId,
                longitude = pvzPoint.Longitude,
                latitude = pvzPoint.Latitude
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task<PvzPoint?> Get(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  SELECT
                  id AS Id, 
                      pvz_id AS PvzId,
                      longitude AS Longitude,
                      latitude AS Latitude
                  FROM pvz_points 
                  WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);
        
        var dao = await connection.QueryFirstOrDefaultAsync<PvzPointDao>(command);
        return dao?.ToDomain();
    }

    public async Task Update(PvzPoint pvzPoint, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                    UPDATE pvz_points SET pvz_id = @pvzId, longitude = @longitude, latitude = @latitude
                    WHERE id = @pointId
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                pvzId =  pvzPoint.PvzId,
                longitude = pvzPoint.Longitude,
                latitude = pvzPoint.Latitude,
                pointId = pvzPoint.Id
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  DELETE FROM pvz_points WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}