using OrderService.Domain;
using OrderService.Infrastructure.Models;

namespace OrderService.Infrastructure.Mappers;

public static class DaoMapper
{
    public static Order? ToDomain(this OrderDao? dao)
    {
        if (dao is null) return null;
    
        return Order.Restore(
            dao.Id,
            dao.PvzId,
            dao.CreatedOn,
            dao.DeliveryDate,
            dao.Status,
            dao.Amount);
    }
    
    public static OrderItem ToDomain(this OrderItemDao dao)
    {
        return new OrderItem(dao.OrderId, dao.ProductId, dao.Quantity);
    }

    public static IEnumerable<OrderInfo> ToDomain(this IEnumerable<OrderInfoRowDao> rows)
    {
        return rows
            .GroupBy(orderInfo => orderInfo.OrderId)
            .Select(group =>
            {
                var firstRow = group.First();
                var order = Order.Restore(
                    firstRow.OrderId,
                    firstRow.PvzId,
                    firstRow.CreatedOn,
                    firstRow.DeliveryDate,
                    firstRow.Status,
                    firstRow.Amount);
                var items = group
                    .Select(item => new OrderItem(item.OrderId, item.ProductId, item.Quantity))
                    .ToList();

                return new OrderInfo(order, items);
            });
    }
}