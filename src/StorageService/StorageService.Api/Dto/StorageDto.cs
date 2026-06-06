namespace StorageService.Api.Dto;

public record StorageDto : BaseDto
{
    public string Address { get; init; }
    
    public Guid PointId { get; init; }
};