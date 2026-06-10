namespace StorageService.Application.Dto;

public record AddStoragePointDto : BaseDto
{
    public Guid StorageId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
};