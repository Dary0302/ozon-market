using OrderService.Domain;
using OrderService.Infrastructure.Models;

namespace OrderService.Infrastructure.Mappers;

public static class DaoMapper
{
    public static Order ToDomain(this OrderDao dao)
    {
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

    public static List<OrderInfo> ToDomain(this IEnumerable<OrderInfoRowDao> rows)
    {
        return rows
            .GroupBy(o => o.OrderId)
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
                    .Select(i => new OrderItem(i.OrderId, i.ProductId, i.Quantity))
                    .ToList();

                return new OrderInfo(order, items);
            })
            .ToList();
    }
}