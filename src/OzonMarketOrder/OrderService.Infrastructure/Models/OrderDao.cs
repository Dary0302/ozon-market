using OrderService.Domain;

namespace OrderService.Infrastructure.Models;

public record OrderDao : BaseDao
{
    public Guid PvzId { get; init; }
    
    public DateTime Date { get; init; }
    
    public Status Status { get; init; }
    
    public double Amount { get; init; }
}