namespace StorageService.Infrastructure.Models;

public record StorageDao : BaseDao
{
    public string Address { get; init; }
    public Guid PointId { get; init; }
}