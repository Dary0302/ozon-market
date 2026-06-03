using System.Data;
using Core.Common.DbHelpers.Interfaces;
using Dapper;
using OrderService.Application.Interfaces;
using OrderService.Domain;
using OrderService.Infrastructure.Models;
using OrderService.Infrastructure.Mappers;

namespace OrderService.Infrastructure.Implementations;

public class OrderItemRepository(IPostgresConnectionFactory connectionFactory) : IOrderItemRepository
{
    public async Task<Guid> Add(List<OrderItem> orderItems, IDbConnection dbConnection, IDbTransaction dbTransaction)
    {
        var sql = "INSERT INTO order_items (order_id, product_id, quantity) " +
                  "SELECT @orderId, unnest(@productIds), unnest(@quantities) " +
                  "ON CONFLICT (order_id, product_id) DO NOTHING";

        var rows = await dbConnection.ExecuteAsync(sql, new
        {
            orderId = orderItems.First().OrderId,
            productIds = orderItems.Select(item => item.ProductId).ToArray(),
            quantities = orderItems.Select(item => item.Quantity).ToArray()
        }, dbTransaction);
        
        if (rows != orderItems.Count())
            throw new InvalidOperationException($"Заказ содержит дублирующие позиции");

        return orderItems.First().OrderId;
    }

    public async Task<IEnumerable<OrderItem>> GetAllByOrderId(Guid orderId)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT product_id, quantity " +
                  "FROM order_items " +
                  "WHERE order_id = @orderId";
        var daos = (await connection.QueryAsync<OrderItemDao>(sql, new { orderId }));
        var items = daos.Select(dao => dao.ToDomain());
        return items;
    }
}