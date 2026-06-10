namespace StorageService.Application.Dto;

public record AddPvzDto : BaseDto
{
    public string Address { get; init; }
    
    public Guid PointId { get; init; }
};