using OrderService.Domain;

namespace OrderService.Infrastructure.Models;

public record OrderDao : BaseDao
{
    public Guid PvzId { get; init; }
    
    public DateTime CreatedOn { get; init; }
    
    public DateTime DeliveryDate { get; init; }
    
    public Status Status { get; init; }
    
    public double Amount { get; init; }
}