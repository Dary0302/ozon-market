using FluentAssertions;
using FluentResults;
using Moq;
using NUnit.Framework;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;

namespace ProductService.Tests.UnitTests;

public class PhotoServiceTests
{
    private Mock<IS3StorageService> s3Mock = null!;
    private PhotoService photoService = null!;

    [SetUp]
    public void SetUp()
    {
        s3Mock = new Mock<IS3StorageService>();
        photoService = new PhotoService(s3Mock.Object);
    }

    [Test]
    public async Task AddPhotoAsync_ShouldReturnPhotoId()
    {
        var photoId = Guid.NewGuid();

        s3Mock.Setup(service =>
                service.SaveImageAsync(It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(photoId));

        var result = await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = [1, 2, 3] },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(photoId);
    }

    [Test]
    public async Task AddPhotoAsync_ShouldReturnFailure_WhenS3Fails()
    {
        s3Mock.Setup(service =>
                service.SaveImageAsync(It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail("error"));

        var result = await photoService.AddPhotoAsync(new AddPhotoDto { PhotoData = [1, 2, 3] },
            CancellationToken.None);

        result.IsFailed.Should().BeTrue();
    }

    [Test]
    public async Task GetPhotoLink_ShouldReturnLink()
    {
        var photoId = Guid.NewGuid();

        s3Mock.Setup(service =>
                service.GetPreSignedUrl(photoId,
                    It.IsAny<TimeSpan>()))
            .ReturnsAsync(Result.Ok("https://test"));

        var result = await photoService.GetPhotoLinkByIdAsync(photoId,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.DownloadPath.Should().Be("https://test");
    }

    [Test]
    public async Task DeletePhoto_ShouldReturnSuccess()
    {
        s3Mock.Setup(service =>
                service.DeleteImageAsync(It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var result = await photoService.DeletePhotoByIdAsync(Guid.NewGuid(),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}