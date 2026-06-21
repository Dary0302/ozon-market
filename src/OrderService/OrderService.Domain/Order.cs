using Core.Common.Errors;
using FluentResults;
using OrderService.Domain.Exseptions;

namespace OrderService.Domain;

public record Order : BaseEntity
{
    public Guid PvzId { get; init; }
    
    public DateTime CreatedOn { get; init; }
    
    public DateTime DeliveryDate { get; init; }
    
    public Status Status { get; private set; }
    
    public decimal Amount { get; init; }
    
    public Order()
    {
        
    }
    
    public Order(decimal amount, Guid pvzId, DateTime deliveryDate)
    {
        PvzId = pvzId;
        CreatedOn = DateTime.UtcNow;
        Status = Status.Created;
        Amount = amount;
        DeliveryDate = deliveryDate;
    }

    public static Order Restore(Guid id, Guid pvzId, DateTime createdOn, DateTime deliveryDate, Status status, decimal amount)
    {
        return new Order
        {
            Id = id,
            PvzId = pvzId,
            CreatedOn = createdOn,
            DeliveryDate = deliveryDate,
            Status = status,
            Amount = amount
        };
    }

    public Result Pay() => Transition(Status.Created, Status.Paid, OrderErrors.MustBeCreated);
    
    public Result Collect() => Transition(Status.Paid, Status.InAssembly,  OrderErrors.MustBePaid);
    
    public Result TransferForDelivery() => 
        Transition(Status.InAssembly, Status.TransferredForDelivery, OrderErrors.MustBeCollected);

    public Result Complete() => 
        Transition(Status.TransferredForDelivery, Status.Delivered, OrderErrors.MustBeTransferredForDelivery);
    
    public Result Cancel()
    {
        if (Status != Status.Delivered)
        {
            Status = Status.Canceled;
            return Result.Ok();
        }
        return Result.Fail(OrderErrors.InvalidStateForCancel());
    }
    
    private Result Transition(Status expected, Status next, Func<AppError> errorFactory)
    {
        if (Status != expected)
            return Result.Fail(errorFactory());

        Status = next;
        return Result.Ok();
    }
}