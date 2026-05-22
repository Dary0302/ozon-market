using Core.Common.Errors;
using FluentResults;
using OzonMarketOrder.Domain.Exseptions;

namespace OzonMarketOrder.Domain;

public class Order : BaseEntity
{
    public Guid PvzId { get; init; }
    
    public DateTime CreatedOn { get; init; }
    
    public Status Status { get; private set; }
    
    public Order()
    {
        PvzId = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
        Status = Status.Created;
    }

    public Result Pay()
    {
        if (Status != Status.Created)
            return Result.Fail(OrderErrors.MustBeCreated());
        Status = Status.Paid;
        return Result.Ok();
    }
    
    public Result Collect()
    {
        if (Status != Status.Paid)
            return Result.Fail(OrderErrors.MustBePaid());
        Status = Status.InAssembly;
        return Result.Ok();
    }
    
    public Result TransferForDelivery()
    {
        if (Status != Status.InAssembly)
            return Result.Fail(OrderErrors.MustBeCollected());
        Status = Status.TransferredForDelivery;
        return Result.Ok();
    }
    
    public Result Complete()
    {
        if (Status != Status.TransferredForDelivery)
            return Result.Fail(OrderErrors.MustBeTransferredForDelivery());
        Status = Status.Delivered;
        return Result.Ok();
    }
    
    public Result Cancel()
    {
        if (Status == Status.InAssembly || Status == Status.TransferredForDelivery)
        {
            Status = Status.Canceled;
            return Result.Ok();
        }
        return Result.Fail(OrderErrors.InvalidStateForCancel(Status.ToString().ToLower()));
    }
}