namespace StorageService.Infrastructure.Models;

public record PvzDao : BaseDao
{
    public string Address { get; init; } 
    
    public Guid PointId { get; init; }
};