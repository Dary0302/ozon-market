using Core.Common.Errors;

namespace OrderService.Domain.Exseptions;

public class OrderErrors
{
    public static AppError InvalidStatusForPay()
        => AppError.Conflict("Заказ уже оплачен");
    public static AppError MustBePaid()
        => AppError.Conflict("Заказ должен быть оплачен");
    public static AppError MustBeCollected()
        => AppError.Conflict("Заказ должен быть собран");
    public static AppError MustBeTransferredForDelivery()
        => AppError.Conflict("Заказ должен быть передан в доставку");
    public static AppError InvalidStateForCancel()
        => AppError.Conflict($"Доставленный заказ не может быть отменен");
    public static AppError InsufficientStock(IEnumerable<LackingProduct> products)
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