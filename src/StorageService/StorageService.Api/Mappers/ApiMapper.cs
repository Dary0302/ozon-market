using StorageService.Application.Dto;
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

    public static AddPvzDto ToHttp(this Pvz pvz)
    {
        return new AddPvzDto
        {
            Id = pvz.Id,
            Address = pvz.Address,
            PointId = pvz.PointId,
        };
    }
    
    public static AddStorageDto ToHttp(this Storage storage)
    {
        return new AddStorageDto
        {
            Id = storage.Id,
            Address = storage.Address,
            PointId = storage.PointId,
        };
    }
    
    public static AddStoragePointDto ToHttp(this StoragePoint point)
    {
        return new AddStoragePointDto
        {
            Id = point.Id,
            StorageId = point.StorageId,
            Longitude = point.Longitude,
            Latitude = point.Latitude
        };
    }
    
    public static AddPvzPointDto ToHttp(this PvzPoint point)
    {
        return new AddPvzPointDto
        {
            Id = point.Id,
            PvzId = point.PvzId,
            Longitude = point.Longitude,
            Latitude = point.Latitude
        };
    }
}