using Dapper;
using Core.Common.DbHelpers.Interfaces;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Infrastructure.Models;
using OrderService.Infrastructure.Mappers;

namespace OrderService.Infrastructure.Implementations;

public class OrderRepository(IPostgresConnectionFactory connectionFactory) : IOrderRepository
{
    public async Task<Guid> Create(Order order)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "INSERT INTO orders (id, pvz_id, created_on, status, dalivery_date, amount) " +
                  "VALUES (@id, @pvz_id, @createdOn, @status, @deliveryDate, @amount)";

        var rows = await connection.ExecuteAsync(sql, new
        {
            id = order.Id,
            pvz_id = order.PvzId,
            createdOn = order.CreatedOn,
            status = order.Status,
            deliveryDate = order.DeliveryDate,
            amount = order.Amount
        });

        return order.Id;
    }

    public async Task<Order?> GetById(Guid id)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT id, pvz_id, created_on, status, delivery_date, amount " +
                  "FROM orders " +
                  "WHERE id = @id";
        var dao = await connection.QueryFirstOrDefaultAsync<OrderDao>(sql, new { id });
        return dao?.ToDomain();
    }

    public async Task<PagedResult<Order>> GetAll(int pageNumber, int pageSize)
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
        await using var multiple = await connection.QueryMultipleAsync(sql, new {skip, pageSize});
        
        var daos = (await multiple.ReadAsync<OrderDao>()).ToList();
        var items = daos.Select(dao => dao.ToDomain()).ToList();
        var total = await multiple.ReadFirstAsync<int>();
        
        return new PagedResult<Order>(items, total);
    }

    public async Task<Guid> Save(Order order)
    {
        await using var connection =  connectionFactory.GetConnection();
        
        var sql = "UPDATE orders " +
                  "SET status = @status " +
                  "WHERE id = @id";
        
        var rows = await connection.ExecuteAsync(sql, new
        {
            status = order.Status, 
            id = order.Id,
        });
        if (rows == 0)
            throw new KeyNotFoundException($"Заказ с id {order.Id} не найден");
        return order.Id;
    }

    public async Task Delete(Guid id)
    {
        await using var connection = connectionFactory.GetConnection();

        var sql = "DELETE FROM orders WHERE id = @id";
        var rows = await connection.ExecuteAsync(sql, new { id });

        if (rows == 0)
            throw new KeyNotFoundException($"Заказ с id {id} не найден");
    }
}