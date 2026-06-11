using System.Data;
using Dapper;
using Core.Common.DbHelpers.Interfaces;
using FluentResults;
using Npgsql;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Infrastructure.Models;
using OrderService.Infrastructure.Mappers;

namespace OrderService.Infrastructure.Implementations;

public class OrderRepository(IPostgresConnectionFactory connectionFactory) : IOrderRepository
{
    public async Task<Guid> Create(Order order, IDbConnection dbConnection, 
        IDbTransaction dbTransaction, CancellationToken cancellationToken)
    { 
        var sql = "INSERT INTO orders (id, pvz_id, created_on, status, delivery_date, amount) " +
                  "VALUES (@id, @pvz_id, @createdOn, @status, @deliveryDate, @amount)";

        var command = new CommandDefinition(sql, new
        {
            id = order.Id,
            pvz_id = order.PvzId,
            createdOn = order.CreatedOn,
            status = order.Status,
            deliveryDate = order.DeliveryDate,
            amount = order.Amount
        }, transaction: dbTransaction, cancellationToken: cancellationToken);
        
        var rows = await dbConnection.ExecuteAsync(command);

        return order.Id;
    }

    public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT id, pvz_id, created_on, status, delivery_date, amount " +
                  "FROM orders " +
                  "WHERE id = @id";
        var command = new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken);
        
        var dao = await connection.QueryFirstOrDefaultAsync<OrderDao>(command);
        return dao?.ToDomain();
    }

    public async Task<PagedResult<Order>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql =
            "SELECT id, pvz_id, created_on, status, delivery_date, amount " +
            "FROM orders " +
            "ORDER BY created_on DESC " +
            "OFFSET @skip " +
            "LIMIT @pageSize; " +
            "SELECT COUNT(1) " +
            "FROM orders;";
        
        var skip = (pageNumber - 1) * pageSize;
        var command = new CommandDefinition(sql, new {skip, pageSize}, cancellationToken: cancellationToken);
        
        await using var multiple = await connection.QueryMultipleAsync(command);
        
        var daos = (await multiple.ReadAsync<OrderDao>());
        var items = daos.Select(dao => dao.ToDomain());
        var total = await multiple.ReadFirstAsync<int>();
        
        return new PagedResult<Order>(items, total);
    }

    public async Task<Guid> Save(Order order, IDbConnection dbConnection, 
        IDbTransaction dbTransaction, CancellationToken cancellationToken)
    {
        var sql = "UPDATE orders " +
                  "SET status = @status " +
                  "WHERE id = @id";
        var command = new CommandDefinition(sql, new
        {
            status = order.Status, 
            id = order.Id,
        }, transaction: dbTransaction, cancellationToken: cancellationToken);
        
        var rows = await dbConnection.ExecuteAsync(command);
        
        if (rows == 0)
            throw new KeyNotFoundException($"Заказ с id {order.Id} не найден");
        
        return order.Id;
    }

    public async Task Delete(Guid id, IDbConnection dbConnection,
        IDbTransaction dbTransaction, CancellationToken cancellationToken)
    {
        var sql = "DELETE FROM orders WHERE id = @id";
        var command = new CommandDefinition(sql, new { id }, 
            transaction: dbTransaction, cancellationToken: cancellationToken);
        var rows = await dbConnection.ExecuteAsync(command);

        if (rows == 0)
            throw new KeyNotFoundException($"Заказ с id {id} не найден");
    }
}