using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Dto;

public record AddPhotoDto
{
    public string PhotoData { get; init; }
}