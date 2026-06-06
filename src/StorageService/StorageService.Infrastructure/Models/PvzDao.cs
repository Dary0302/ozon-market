namespace StorageService.Infrastructure.Models;

public record PvzDao(string Address, Guid PointId) : BaseDao;