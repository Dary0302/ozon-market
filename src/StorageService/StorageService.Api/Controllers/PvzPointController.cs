using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Dto;
using StorageService.Api.Mappers;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Api.Controllers;

[ApiController]
[Route("api/pvz-points")]
public class PvzPointController(IPvzPointService service) : ControllerBase
{
    /// <summary>
    /// Добавление информации о местоположении пункта выдачи заказов
    /// </summary>
    /// <param name="addPvzPoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddPvzPoint([FromBody] AddPvzPointDto addPvzPoint, CancellationToken cancellationToken)
    {
        var result = await service.AddPvzPoint(addPvzPoint, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Получение информации о местоположении пункта выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddPvzPointDto>> GetPvzPoint(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetPvzPoint(id, cancellationToken);
        return result.ToActionResult(point => point.ToHttp());
    }

    /// <summary>
    /// Обновление информации о местоположении пункта выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="addPvzPoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePvzPoint(Guid id, [FromBody] AddPvzPointDto addPvzPoint, CancellationToken cancellationToken)
    {
        var result = await service.UpdatePvzPoint(id, addPvzPoint, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Удаление информации о местоположении пункта выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePvzPoint(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeletePvzPoint(id, cancellationToken);
        return result.ToActionResult();
    }
}