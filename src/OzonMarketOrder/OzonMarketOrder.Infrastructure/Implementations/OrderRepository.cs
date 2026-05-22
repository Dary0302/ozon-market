using Dapper;
using Core.Common.DbHelpers;
using Core.Common.Errors;
using FluentResults;
using Npgsql;
using OzonMarketOrder.Application.Interfaces;
using OzonMarketOrder.Application.Models;
using OzonMarketOrder.Domain;
using OzonMarketOrder.Infrastructure.Mappers;
using OzonMarketOrder.Infrastructure.Models;

namespace OzonMarketOrder.Infrastructure.Implementations;

public class OrderRepository(IPostgresConnectionFactory connectionFactory) : IOrderRepository
{
    public async Task<Result<Guid>> Create(Order order)
    {
        await using var connection = connectionFactory.GetConnection();

        try
        {
            var sql = "INSERT INTO orders (id, pvz_id, status, date, amount) " +
                      "VALUES (@id, @pvz_id, @status, @date, @amount)";

            var rows = await connection.ExecuteAsync(sql, new
            {
                id = order.Id,
                pvz_id = order.PvzId,
                status = order.Status,
                date = order.CreatedOn,
                amount = order.Amount
            });

            return Result.Ok(order.Id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation &&
                                           ex.ConstraintName == "pk_orders")
        {
            return Result.Fail(AppError.Conflict($"Заказ с id {order.Id} уже существует"));
        }
    }

    public async Task<Result<Order>> GetById(Guid id)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT id, pvz_id, status, date, amount " +
                  "FROM orders " +
                  "WHERE id = @id";
        var dao = await connection.QueryFirstOrDefaultAsync<OrderDao>(sql, new { id });
        if (dao == null)
            return Result.Fail(AppError.NotFound($"Заказ с id {id} не найден"));
        return Result.Ok(dao.ToDomain());
    }

    public async Task<Result<OrdersPagedResult<Order>>> GetAll(int pageNumber, int pageSize)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT id, pvz_id, status, date, amount " +
                  "FROM orders " +
                  "OFFSET @skip " +
                  "LIMIT @pageSize; " +
                  "SELECT COUNT(1) " +
                  "FROM orders;";
        var skip = (pageNumber - 1) * pageSize;
        await using var multiple = await connection.QueryMultipleAsync(sql, new {skip, pageSize});
        
        var daos = (await multiple.ReadAsync<OrderDao>()).ToList();
        var items = daos.Select(dao => dao.ToDomain()).ToList();
        var total = await multiple.ReadFirstAsync<int>();
        
        return Result.Ok(new OrdersPagedResult<Order>(items, total));
    }

    public async Task<Result<Guid>> UpdateStatus(Guid id, Status status)
    {
        await using var connection =  connectionFactory.GetConnection();
        
        var sql = "UPDATE orders " +
                  "SET status = @status " +
                  "WHERE id = @id";
        
        var rows = await connection.ExecuteAsync(sql, new { status, id });
        if (rows == 0)
            return Result.Fail(AppError.NotFound($"Заказ с id {id} не найден"));
        return Result.Ok(id);
    }

    public async Task<Result> Delete(Guid id)
    {
        await using var connection = connectionFactory.GetConnection();

        var sql = "DELETE FROM orders WHERE id = @id";
        var rows = await connection.ExecuteAsync(sql, new { id });
        
        if (rows == 0)
            return Result.Fail(AppError.NotFound($"Заказ с id {id} не найден"));
        
        return Result.Ok();
    }
}