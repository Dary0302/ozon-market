using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Common.Errors;
using FluentResults;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Interfaces;
using SixLabors.ImageSharp;

namespace ProductService.Application.Services;

public class S3StorageService : IS3StorageService
{
    private readonly string bucketName;
    private readonly string path;
    private readonly AmazonS3Client s3Client;

    private const string FileExtension = ".webp";   

    public S3StorageService(IConfiguration config)
    {
        bucketName = config["YandexS3:BucketName"];
        path = config["YandexS3:Path"];
        var amazonConfig = new AmazonS3Config { ServiceURL = "https://s3.yandexcloud.net", ForcePathStyle = true };
        s3Client = new AmazonS3Client(
            new BasicAWSCredentials(config["YandexS3:AccessKey"], config["YandexS3:SecretKey"]),
            amazonConfig);
    }

    public async Task<Result<Guid>> SaveImageAsync(Stream stream, CancellationToken cancellationToken)
    {
        var photoId = Guid.NewGuid();
        var key = GetKey(photoId);  

        var saveFileResult = await SaveFileAsync(key, stream, cancellationToken);
        return saveFileResult.IsSuccess ? Result.Ok(photoId) : Result.Fail(AppError.UnprocessableContent());
    }

    public async Task<Result<string>> GetPreSignedUrl(Guid photoId, TimeSpan validFor)
    {
        var key = GetKey(photoId);
          
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName, Key = key, Expires = DateTime.UtcNow.Add(validFor), Verb = HttpVerb.GET
        };

        var getResult = await s3Client.GetPreSignedURLAsync(request);

        return getResult is null 
            ? Result.Fail(AppError.NotFound()) 
            : Result.Ok(getResult);
    }

    public async Task<Result> DeleteImageAsync(Guid photoId, CancellationToken cancellationToken)
    {
        var key = GetKey(photoId);               
                               
        var removeRequest = new DeleteObjectRequest { BucketName = bucketName, Key = key };

        var deleteResult = await s3Client.DeleteObjectAsync(removeRequest, cancellationToken);
        
        return Result.OkIf(deleteResult is not null, AppError.NotFound());
    }

    private string GetKey(Guid photoId)
    {
        var name = $"{photoId}{FileExtension}";
        var key = $"{path}/{name}";
        
        return key;
    }

    private async Task<Result> SaveFileAsync(
        string key,
        Stream stream,
        CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(stream, cancellationToken);

        await using var webpStream = new MemoryStream();
        await image.SaveAsWebpAsync(webpStream, cancellationToken);

        webpStream.Seek(0, SeekOrigin.Begin);

        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = webpStream,
            ContentType = "image/webp",
            CannedACL = S3CannedACL.PublicRead,
            DisablePayloadSigning = true
        };

        var response = await s3Client.PutObjectAsync(putRequest, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK ? Result.Ok() : Result.Fail(AppError.Conflict());
    }
}