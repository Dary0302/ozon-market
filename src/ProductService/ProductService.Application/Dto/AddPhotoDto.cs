namespace ProductService.Application.Dto;

public record AddPhotoDto
{
    public byte[] PhotoData { get; init; }
}