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
    public async Task Add(Pvz pvz)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "INSERT INTO pvz (id, address, pointId)" +
                  "VALUES (@id, @address, @pointId)";
        
        await connection.ExecuteAsync(sql, new
        {
            id = pvz.Id,
            address = pvz.Address,
            pointId = pvz.PointId
        });
    }

    public async Task<Pvz?> Get(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = @"SELECT 
                        id AS Id, 
                        address AS Address,
                        pointId AS PointId
                    FROM pvz 
                    WHERE id = @id";
        
        var dao = await connection.QueryFirstOrDefaultAsync<PvzDao>(sql, new { id });
        return dao?.ToDomain();
    }

    public async Task<IEnumerable<Pvz>> GetAll()
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "SELECT id, address, pointId FROM pvz";
        
        var daos = await connection.QueryAsync<PvzDao>(sql);
        
        var allPvz = daos.Select(dao => dao.ToDomain());

        return allPvz;
    }

    public async Task Update(Pvz pvz)
    {
        await using var connection = postgresConnectionFactory.GetConnection();

        var sql = "UPDATE pvz SET address = @address, pointId = @pointId WHERE id = @id";
        
        await connection.ExecuteAsync(sql, new
        {
            address = pvz.Address,
            pointId = pvz.PointId,
            id = pvz.Id
        });
    }

    public async Task Delete(Guid id)
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "DELETE FROM pvz WHERE id = @id";

        await connection.ExecuteAsync(sql, new { id });
    }
}