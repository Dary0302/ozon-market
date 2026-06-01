using FluentResults;
using ProductService.Application.Dto;

namespace ProductService.Application.Interfaces;

public interface IPhotoService
{
    Task<Result<Guid>> AddPhotoAsync(
        AddPhotoDto addPhotoDto,
        CancellationToken cancellationToken
    );

    Task<Result<GetPhotoLinkDto>> GetPhotoLinkByIdAsync(
        Guid photoId,
        CancellationToken cancellationToken
    );

    Task<Result> DeletePhotoByIdAsync(
        Guid photoId,
        CancellationToken cancellationToken
    );
}