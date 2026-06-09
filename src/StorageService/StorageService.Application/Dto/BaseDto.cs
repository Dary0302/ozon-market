namespace StorageService.Application.Dto;

public record BaseDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
}