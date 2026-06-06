using StorageService.Api.Dto;
using StorageService.Domain;

namespace StorageService.Api.Mappers;

public static class ApiMapper
{
    public static ProductQuantityDto ToHttp(this ProductQuantity quantity)
    {
        return new ProductQuantityDto(quantity.ProductId, quantity.Quantity);
    }

    public static DecreaseQuantityDto ToHttp(this DecreaseQuantity quantity)
    {
        return new DecreaseQuantityDto(quantity.ProductId, quantity.StorageId, quantity.Quantity);
    }

    public static PvzDto ToHttp(this Pvz pvz)
    {
        return new PvzDto
        {
            Id = pvz.Id,
            Address = pvz.Address,
            PointId = pvz.PointId,
        };
    }
    
    public static StorageDto ToHttp(this Storage storage)
    {
        return new StorageDto
        {
            Id = storage.Id,
            Address = storage.Address,
            PointId = storage.PointId,
        };
    }
    
    public static StoragePointDto ToHttp(this StoragePoint point)
    {
        return new StoragePointDto
        {
            Id = point.Id,
            StorageId = point.StorageId,
            Longitude = point.Longitude,
            Latitude = point.Latitude
        };
    }
    
    public static PvzPointDto ToHttp(this PvzPoint point)
    {
        return new PvzPointDto
        {
            Id = point.Id,
            PvzId = point.PvzId,
            Longitude = point.Longitude,
            Latitude = point.Latitude
        };
    }
}