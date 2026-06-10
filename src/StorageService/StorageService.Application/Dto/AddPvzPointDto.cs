namespace StorageService.Application.Dto;

public record AddPvzPointDto : BaseDto
{
    public Guid PvzId { get; init; }
    
    public double Longitude { get; init; }
    
    public double Latitude { get; init; }
}