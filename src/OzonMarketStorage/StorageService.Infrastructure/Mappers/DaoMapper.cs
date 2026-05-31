using StorageService.Domain;
using StorageService.Infrastructure.Models;

namespace StorageService.Infrastructure.Mappers;

public static class DaoMapper
{
    public static Storage ToDomain(this StorageDao dao)
    {
        return Storage.Restore(dao.Id, dao.Address, dao.PointId);
    }

    public static StoragePoint ToDomain(this StoragePointDao dao)
    {
        return StoragePoint.Restore(dao.Id, dao.StorageId, dao.Longitude, dao.Latitude);
    }

    public static Pvz ToDomain(this PvzDao dao)
    {
        return Pvz.Restore(dao.Id, dao.Address, dao.PointId);
    }

    public static PvzPoint ToDomain(this PvzPointDao dao)
    {
        return PvzPoint.Restore(dao.Id, dao.PvzId, dao.Longitude, dao.Latitude);
    }

    public static StoredProduct ToDomain(this StoredProductDao dao)
    {
        return new StoredProduct(dao.ProductId, dao.StorageId, dao.Quantity);
    }
}