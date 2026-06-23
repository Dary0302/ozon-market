using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/photos")]
public class PhotoController(IPhotoService service, IProductManagementService productService) : ControllerBase
{
    /// <summary>
    /// Получение ссылки на фото по id
    /// </summary>
    /// <param name="productId">Id фото</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("link/{product-id:guid}")]
    public async Task<ActionResult<GetPhotoLinkDto>> Get(
        [FromRoute(Name = "product-id")] Guid productId,
        CancellationToken cancellationToken)
    {
        var photoIdResult = await productService.GetProduct(productId, cancellationToken);
        if (photoIdResult.Value.PhotoId is null)
        {
            return NotFound();
        }

        var photoId = photoIdResult.Value.PhotoId.Value;
        var photoLinkResult = await service.GetPhotoLinkByIdAsync(photoId, cancellationToken);
        return photoLinkResult.ToActionResult();
    }
}