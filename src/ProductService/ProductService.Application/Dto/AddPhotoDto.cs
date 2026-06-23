using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Dto;

public record AddPhotoDto
{
    public IFormFile PhotoData { get; init; }
}