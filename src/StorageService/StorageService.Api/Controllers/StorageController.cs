using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Dto;
using StorageService.Api.Mappers;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Api.Controllers;

[ApiController]
[Route("api/storages")]
public class StorageController(IStorageService service) : ControllerBase
{
    /// <summary>
    /// Добавление информации о новом складе
    /// </summary>
    /// <param name="addStorage"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddStorage([FromBody] AddStorageDto addStorage, CancellationToken cancellationToken)
    {
        var result = await service.AddStorage(addStorage, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Получение информации о складе по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddStorageDto>> GetStorage(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetStorage(id, cancellationToken);
        return result.ToActionResult(storage => storage.ToHttp());
    }

    /// <summary>
    /// Обновление информации о складе
    /// </summary>
    /// <param name="pointId"></param>
    /// <param name="addStorage"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{pointId:guid}")]
    public async Task<ActionResult> UpdateStorage(Guid pointId, [FromBody] AddStorageDto addStorage, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStorage(pointId, addStorage, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Удалении информации о складе
    /// </summary>
    /// <param name="pointId"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{pointId:guid}")]
    public async Task<ActionResult> DeleteStorage(Guid pointId, CancellationToken cancellationToken)
    {
        var result = await service.DeleteStorage(pointId, cancellationToken);
        return result.ToActionResult();
    }
}