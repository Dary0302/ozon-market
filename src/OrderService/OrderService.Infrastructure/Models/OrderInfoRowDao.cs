using OrderService.Application.Interfaces;
using OrderService.Domain;

namespace OrderService.Infrastructure.Models;

public record OrderInfoRowDao
{
    public Guid OrderId { get; init; }

    public Guid PvzId { get; init; }

    public DateTime CreatedOn { get; init; }

    public DateTime DeliveryDate { get; init; }

    public Status Status { get; init; }

    public decimal Amount { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}