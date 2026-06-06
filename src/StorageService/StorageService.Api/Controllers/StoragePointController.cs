using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Api.Dto;
using StorageService.Api.Mappers;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Api.Controllers;

[ApiController]
[Route("api/storage-points")]
public class StoragePointController(IStoragePointService service) : ControllerBase
{
    /// <summary>
    /// Добавление информации о местоположении склада
    /// </summary>
    /// <param name="storagePoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateStoragePoint([FromBody] StoragePoint storagePoint)
    {
        var result = await service.AddStoragePoint(storagePoint);
        return result.ToActionResult();
    }

    /// <summary>
    /// Получение информации о местоположении склада
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoragePointDto>> GetStoragePoint(Guid id)
    {
        var result = await service.GetStoragePoint(id);
        return result.ToActionResult(point => point.ToHttp());
    }

    /// <summary>
    /// Обновление информации о местоположении склада
    /// </summary>
    /// <param name="id"></param>
    /// <param name="storagePoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateStoragePoint(Guid id, [FromBody] StoragePoint storagePoint)
    {
        var result = await service.UpdateStoragePoint(id, storagePoint);
        return result.ToActionResult();
    }

    /// <summary>
    /// Удаление информации о местоположении склада
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteStoragePoint(Guid id)
    {
        var result = await service.DeleteStoragePoint(id);
        return result.ToActionResult();
    }
}