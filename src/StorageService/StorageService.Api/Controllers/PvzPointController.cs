using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Api.Dto;
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
    /// <param name="pvzPoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddPvzPoint([FromBody] PvzPoint pvzPoint)
    {
        var result = await service.AddPvzPoint(pvzPoint);
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
    public async Task<ActionResult<PvzPointDto>> GetPvzPoint(Guid id)
    {
        var result = await service.GetPvzPoint(id);
        return result.ToActionResult(point => point.ToHttp());
    }

    /// <summary>
    /// Обновление информации о местоположении пункта выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pvzPoint"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePvzPoint(Guid id, [FromBody] PvzPoint pvzPoint)
    {
        var result = await service.UpdatePvzPoint(id, pvzPoint);
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
    public async Task<ActionResult> DeletePvzPoint(Guid id)
    {
        var result = await service.DeletePvzPoint(id);
        return result.ToActionResult();
    }
}