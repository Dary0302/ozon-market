using Core.Common.Errors;

namespace OrderService.Domain.Exseptions;

public class OrderErrors
{
    public static AppError MustBeCreated()
        => AppError.Conflict("Заказ должен быть создан");
    public static AppError MustBePaid()
        => AppError.Conflict("Заказ должен быть оплачен");
    public static AppError MustBeCollected()
        => AppError.Conflict("Заказ должен быть собран");
    public static AppError MustBeTransferredForDelivery()
        => AppError.Conflict("Заказ должен быть передан в доставку");
    public static AppError InvalidStateForCancel(string state)
        => AppError.Conflict($"Заказ не может быть отменен в состоянии '{state}'");
    public static AppError InsufficientStock(List<LackingProduct> products)
    {
        var error = new AppError(ErrorStatus.Conflict, "Недостаточно товаров на складе");
        error.Metadata["LackingProducts"] = products;
        return error;
    }
    public static AppError NotFound(Guid id) => 
        AppError.NotFound($"Заказ {id} не найден");
    public static AppError InvalidAmount() => AppError.Conflict("Сумма заказа неактуальна");
    
    public static AppError InvalidStatusTransition() => AppError.Conflict("Неизвестный статус заказа");
}