using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using Core.Common.DbHelpers.Interfaces;
using StorageService.Application.Interfaces.Repositories;
using StorageService.Infrastructure.Mappers;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Implementations;

public class PvzRepository(IPostgresConnectionFactory postgresConnectionFactory) : IPvzRepository
{
    public async Task Add(Pvz pvz, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  INSERT INTO pvz (id, address, point_id)
                  VALUES (@id, @address, @pointId)
                  """;
        
        var command = new CommandDefinition(
            sql,
            new
            {
                id = pvz.Id,
                address = pvz.Address,
                pointId = pvz.PointId
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task<Pvz?> Get(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  SELECT 
                  id AS Id, 
                      address AS Address,
                      point_id AS PointId
                  FROM pvz 
                  WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);
        
        var dao = await connection.QueryFirstOrDefaultAsync<PvzDao>(command);
        return dao?.ToDomain();
    }

    public async Task<IEnumerable<Pvz>> GetAll(CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  SELECT id, address, point_id FROM pvz
                  """;

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);
        
        var daos = await connection.QueryAsync<PvzDao>(command);
        
        var allPvz = daos.Select(dao => dao.ToDomain());

        return allPvz;
    }

    public async Task Update(Pvz pvz, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = """
                  UPDATE pvz SET address = @address, point_id = @pointId WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new
            {
                address = pvz.Address,
                pointId = pvz.PointId,
                id = pvz.Id
            },
            cancellationToken: cancellationToken);
        
        await connection.ExecuteAsync(command);
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = """
                  DELETE FROM pvz WHERE id = @id
                  """;

        var command = new CommandDefinition(
            sql,
            new { id },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}