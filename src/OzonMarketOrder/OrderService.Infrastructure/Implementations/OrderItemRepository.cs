using Core.Common.DbHelpers;
using Core.Common.Errors;
using Dapper;
using FluentResults;
using Npgsql;
using OrderService.Application.Interfaces;
using OrderService.Domain;
using OrderService.Infrastructure.Models;
using OrderService.Infrastructure.Mappers;

namespace OrderService.Infrastructure.Implementations;

public class OrderItemRepository(IPostgresConnectionFactory connectionFactory) : IOrderItemRepository
{
    public async Task<Result<Guid>> Add(OrderItem orderItem)
    {
        await using var connection = connectionFactory.GetConnection();
        try
        {
            var sql = "INSERT INTO order_items (order_id, product_id, quantity) " +
                      "VALUES (@order_id, @product_id, @quantity)";

            var rows = await connection.ExecuteAsync(sql, new
            {
                order_id = orderItem.OrderId,
                product_id = orderItem.ProductId,
                quantity = orderItem.Quantity
            });

            return Result.Ok(orderItem.OrderId);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation &&
                                           ex.ConstraintName == "uq_order_items_order_product")
        {
            return Result.Fail(AppError.Conflict($"Продукт {orderItem.ProductId} уже добавлен в заказ {orderItem.OrderId}"));
        }
    }

    public async Task<Result<List<OrderItem>>> GetAllByOrderId(Guid orderId)
    {
        await using var connection = connectionFactory.GetConnection();
        
        var sql = "SELECT product_id, quantity " +
                  "FROM order_items " +
                  "WHERE order_id = @orderId";
        var daos = (await connection.QueryAsync<OrderItemDao>(sql, new { orderId })).ToList();
        var items = daos.Select(dao => dao.ToDomain()).ToList();
        return Result.Ok(items);
    }
}