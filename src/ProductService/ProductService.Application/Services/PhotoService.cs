using Core.Common.Errors;
using FluentResults;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Services;

public class PhotoService(IS3StorageService s3StorageService) : IPhotoService
{
    private readonly TimeSpan linkLifeTime = TimeSpan.FromHours(3);

    public async Task<Result<Guid>> AddPhotoAsync(AddPhotoDto addPhotoDto, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(addPhotoDto.PhotoData);
        var savePhotoResult = await s3StorageService.SaveImageAsync(stream, cancellationToken);

        return savePhotoResult.IsFailed
            ? Result.Fail(AppError.UnprocessableContent("Ошибка при добавлении фото"))
            : Result.Ok(savePhotoResult.Value);
    }

    public async Task<Result<GetPhotoLinkDto>> GetPhotoLinkByIdAsync(
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var getLinkResult = await s3StorageService.GetPreSignedUrl(photoId, linkLifeTime);

        return getLinkResult.IsFailed
            ? Result.Fail(AppError.NotFound("Фото не найдено"))
            : Result.Ok(new GetPhotoLinkDto { DownloadPath = getLinkResult.Value });
    }

    public async Task<Result> DeletePhotoByIdAsync(Guid photoId, CancellationToken cancellationToken)
    {
        var deletePhotoResult = await s3StorageService.DeleteImageAsync(photoId, cancellationToken);

        return Result.OkIf(deletePhotoResult.IsSuccess, AppError.UnprocessableContent("Ошибка при удалении фото"));
    }
}