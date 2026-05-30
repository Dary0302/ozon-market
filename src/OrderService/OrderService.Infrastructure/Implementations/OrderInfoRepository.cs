using Core.Common.DbHelpers.Interfaces;
using Dapper;
using OrderService.Application.Interfaces;
using OrderService.Application.Models;
using OrderService.Domain;
using OrderService.Infrastructure.Mappers;
using OrderService.Infrastructure.Models;

namespace OrderService.Infrastructure.Implementations;

public class OrderInfoRepository(IPostgresConnectionFactory connectionFactory) : IOrderInfoRepository
{
    public async Task<PagedResult<OrderInfo>> GetAll(int pageNumber, int pageSize)
    {
        await using var connection = connectionFactory.GetConnection();

        var sql =
            "SELECT o.id, o.pvz_id, o.created_on, o.status, o.delivery_date, o.amount, oi.product_id, oi.quantity " +
            "FROM orders AS o " +
            "LEFT JOIN order_items as oi ON oi.order_id = o.id " +
            "WHERE o.id IN ( " +
            "SELECT id " +
            "FROM orders " +
            "ORDER BY created_on DESC " +
            "OFFSET @skip " +
            "LIMIT @pageSize " +
            ") " +
            "ORDER BY o.date DESC; " +
            "SELECT COUNT(1) " +
            "FROM orders;";
        
        var skip = (pageNumber - 1) * pageSize;
        await using var multiple = await connection.QueryMultipleAsync(sql, new {skip, pageSize});
        
        var daos = (await multiple.ReadAsync<OrderInfoRowDao>()).ToList();
        var items = daos.ToDomain();
        var total = await multiple.ReadFirstAsync<int>();
        
        return new PagedResult<OrderInfo>(items, total);
    }
}