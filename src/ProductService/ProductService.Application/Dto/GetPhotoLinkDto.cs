namespace ProductService.Application.Dto;

public record GetPhotoLinkDto
{
    public required string DownloadPath { get; init; } 
}