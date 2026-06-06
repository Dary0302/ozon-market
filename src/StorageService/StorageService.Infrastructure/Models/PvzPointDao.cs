namespace StorageService.Infrastructure.Models;

public record PvzPointDao(Guid PvzId, double Longitude, double Latitude) : BaseDao;