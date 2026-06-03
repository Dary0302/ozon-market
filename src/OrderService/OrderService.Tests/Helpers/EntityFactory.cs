using OrderService.Domain;

namespace OrderService.Tests.Helpers;

public static class EntityFactory
{
    public static Order MakeOrder() 
        => new Order((decimal)1000, Guid.NewGuid(), DateTime.Today + TimeSpan.FromDays(1));

    public static OrderItem MakeOrderItem(Guid orderId)
        => new OrderItem(orderId, Guid.NewGuid(), 100);

    public static OrderInfo MakeOrderInfo()
    {
        var order = MakeOrder();
        var items = Enumerable.Range(0, 5).Select(_ => MakeOrderItem(order.Id));
        return new OrderInfo(order, items);
    }
}