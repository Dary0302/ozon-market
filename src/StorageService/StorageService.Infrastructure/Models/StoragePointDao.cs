namespace StorageService.Infrastructure.Models;

public record StoragePointDao : BaseDao
{
    public Guid StorageId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
};