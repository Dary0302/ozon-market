namespace StorageService.Api.Dto;

public record StoragePointDto : BaseDto
{
    public Guid StorageId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
};