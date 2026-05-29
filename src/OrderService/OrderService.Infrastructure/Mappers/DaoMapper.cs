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
        return new OrderItem(dao.OderId, dao.ProductId, dao.Quantity);
    }
}