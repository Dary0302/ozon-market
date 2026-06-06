namespace StorageService.Infrastructure.Models;

public record StoragePointDao(Guid StorageId, double Longitude, double Latitude) : BaseDao;