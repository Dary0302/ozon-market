namespace StorageService.Api.Dto;

public record PvzPointDto : BaseDto
{
    public Guid PvzId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
}