namespace StorageService.Infrastructure.Models;

public record PvzPointDao : BaseDao
{
    public Guid PvzId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
};