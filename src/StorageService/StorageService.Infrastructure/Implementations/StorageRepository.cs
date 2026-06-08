using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class StorageRepository(IPostgresConnectionFactory postgresConnectionFactory) : IStorageRepository
{
    public async Task Add(Storage storage, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO storages (id, address, point_id)
                  VALUES (@id, @address, @pointId)
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                id = storage.Id,
                address = storage.Address,
                pointId = storage.PointId
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task<Storage?> Get(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        var sql = """
                  SELECT
                  id AS Id, 
                      address AS Address,
                      point_id AS PointId
                  FROM storages 
                  WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);
        
        var dao = await connection.QueryFirstOrDefaultAsync<StorageDao>(command);
        
        return dao?.ToDomain();
    }

    public async Task Update(Storage storage, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  UPDATE storages SET address = @address, point_id = @pointId WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                address = storage.Address,
                pointId = storage.PointId,
                id = storage.Id
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  DELETE FROM storages WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}