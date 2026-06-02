using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;

namespace ProductService.Api;

[ApiController]
[Route("api/photos")]
public class PhotoController(IPhotoService service) : ControllerBase
{
    /// <summary>
    /// Получение ссылки на фото по id
    /// </summary>
    /// <param name="photoId">Id фото</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("link/{photo-id:guid}")]
    public async Task<ActionResult<GetPhotoLinkDto>> Get(
        [FromRoute(Name = "photo-id")] Guid photoId,
        CancellationToken cancellationToken)
    {
        var photoLinkResult = await service.GetPhotoLinkByIdAsync(photoId, cancellationToken);
        return photoLinkResult.ToActionResult();
    }
}