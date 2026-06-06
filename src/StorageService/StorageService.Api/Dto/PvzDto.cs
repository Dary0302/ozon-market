namespace StorageService.Api.Dto;

public record PvzDto : BaseDto
{
    public string Address { get; init; }
    
    public Guid PointId { get; init; }
};