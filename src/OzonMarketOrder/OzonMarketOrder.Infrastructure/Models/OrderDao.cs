using OzonMarketOrder.Domain;

namespace OzonMarketOrder.Infrastructure.Models;

public class OrderDao : BaseDao
{
    public Guid PvzId { get; protected set; }
    
    public DateTime Date { get; protected set; }
    
    public Status Status { get; protected set; }
    
    public double Amount { get; protected set; }
}