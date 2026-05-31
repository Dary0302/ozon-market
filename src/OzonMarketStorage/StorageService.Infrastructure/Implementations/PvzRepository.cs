using Dapper;
using StorageService.Application.Interfaces;
using StorageService.Domain;
using Core.Common.DbHelpers;
using StorageService.Application.Interfaces.Repositories;

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
        
        var pvz = await connection.QueryFirstOrDefaultAsync<Pvz>(sql, new { id });
        return pvz;
    }

    public async Task<IEnumerable<Pvz>> GetAll()
    {
        await using var connection = postgresConnectionFactory.GetConnection();
        
        var sql = "SELECT id, address, pointId FROM pvz";
        
        var allPvz = await connection.QueryAsync<Pvz>(sql);

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