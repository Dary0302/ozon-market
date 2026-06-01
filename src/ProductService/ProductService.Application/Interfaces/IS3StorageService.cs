using FluentResults;

namespace ProductService.Application.Interfaces;

public interface IS3StorageService
{
    Task<Result<Guid>> SaveImageAsync(
        Stream stream,
        CancellationToken cancellationToken
    );

    Task<Result<string>> GetPreSignedUrl(Guid photoId, TimeSpan validFor);

    Task<Result> DeleteImageAsync(Guid photoId, CancellationToken cancellationToken);
}