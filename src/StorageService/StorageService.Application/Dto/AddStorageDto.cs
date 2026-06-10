namespace StorageService.Application.Dto;

public record AddStorageDto : BaseDto
{
    public string Address { get; init; }
    
    public Guid PointId { get; init; }
};