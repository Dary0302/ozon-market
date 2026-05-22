using Core.Common.Errors;
using FluentResults;
using OzonMarketOrder.Domain.Exseptions;

namespace OzonMarketOrder.Domain;

public class Order : BaseEntity
{
    public Guid PvzId { get; init; }
    
    public DateTime CreatedOn { get; init; }
    
    public Status Status { get; protected set; }
    
    public double Amount { get; init; }
    
    public Order()
    {
        
    }
    
    public Order(double amount, Guid pvzId)
    {
        Id = Guid.NewGuid();
        PvzId = pvzId;
        CreatedOn = DateTime.UtcNow;
        Status = Status.Created;
    }

    public static Order Restore(Guid id, Guid pvzId, DateTime createdOn, Status status, double amount)
    {
        return new Order
        {
            Id = id,
            PvzId = pvzId,
            CreatedOn = createdOn,
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
        if (Status == Status.InAssembly || Status == Status.TransferredForDelivery)
        {
            Status = Status.Canceled;
            return Result.Ok();
        }
        return Result.Fail(OrderErrors.InvalidStateForCancel(Status.ToString()));
    }
    
    private Result Transition(Status expected, Status next, Func<AppError> errorFactory)
    {
        if (Status != expected)
            return Result.Fail(errorFactory());

        Status = next;
        return Result.Ok();
    }
}