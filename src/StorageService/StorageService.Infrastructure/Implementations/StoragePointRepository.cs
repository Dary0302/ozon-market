using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class StoragePointRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStoragePointRepository
{
    public async Task Add(StoragePoint storagePoint, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO storagePoints (pointId, storageId, longitude, latitude)
                  VALUES (@pointId, @storageId, @longitude, @latitude)
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                pointId = storagePoint.Id,
                storageId = storagePoint.StorageId,
                longitude = storagePoint.Longitude,
                latitude = storagePoint.Latitude
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task<StoragePoint?> Get(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  SELECT 
                  pointId AS Id, 
                      storageId AS StorageId,
                      longitude AS Longitude,
                      latitude AS Latitude
                  FROM storagePoints 
                  WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);
        
        var dao = await connection.QueryFirstOrDefaultAsync<StoragePointDao>(command);
        
        return dao?.ToDomain();
    }

    public async Task<IEnumerable<StoragePoint>> GetStoragePoints(IEnumerable<Guid> storageIds, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  SELECT
                  pointId AS Id, 
                      storageId AS StorageId,
                      longitude AS Longitude,
                      latitude AS Latitude
                  FROM storagePoints 
                  WHERE id IN @storageIds 
                  """;

        var command = new CommandDefinition(
            sql,
            new { storageIds },
            cancellationToken: cancellationToken);
        
        var daos = await connection.QueryAsync<StoragePointDao>(command);

        var storagePoints = daos.Select(dao => dao.ToDomain());
        
        return storagePoints;
    }

    public async Task Update(StoragePoint storagePoint, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  UPDATE storagePoints SET storageId = @storageId, longitude = @longitude, latitude = @latitude
                  WHERE pointId = @pointId
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                storageId =  storagePoint.StorageId,
                longitude = storagePoint.Longitude,
                latitude = storagePoint.Latitude,
                pointId = storagePoint.Id
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  DELETE FROM storagePoints WHERE pointId = @pointId
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}