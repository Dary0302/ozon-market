namespace StorageService.Infrastructure.Models;

public record StorageDao(string Address, Guid PointId) : BaseDao;