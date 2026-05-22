using Core.Common.Errors;

namespace OzonMarketOrder.Domain.Exseptions;

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
}