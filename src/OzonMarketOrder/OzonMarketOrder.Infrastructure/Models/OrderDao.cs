using OzonMarketOrder.Domain;

namespace OzonMarketOrder.Infrastructure.Models;

public class OrderDao : BaseDao
{
    public Guid PvzId { get; protected set; }
    
    public DateTime CreatedOn { get; protected set; }
    
    public Status Status { get; protected set; }
}