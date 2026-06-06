using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Dto;
using StorageService.Api.Mappers;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Api.Controllers;

[ApiController]
[Route("api/pvz")]
public class PvzController(IPvzService service) : ControllerBase
{
    /// <summary>
    /// Получение всех пунктов выдачи заказов
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<AddPvzDto>>> GetAllPvz(CancellationToken cancellationToken)
    {
        var result = await service.GetAllPvz(cancellationToken);
        return result.ToActionResult(allPvz => allPvz.Select(pvz => pvz.ToHttp()));
    }

    /// <summary>
    /// Получение пункта выдачи по id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddPvzDto>> GetPvz(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetPvz(id, cancellationToken);
        return result.ToActionResult(pvz => pvz.ToHttp());
    }

    /// <summary>
    /// Добавление нового пункта выдачи заказов
    /// </summary>
    /// <param name="addPvz"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddPvz([FromBody] AddPvzDto addPvz, CancellationToken cancellationToken)
    {
        var result = await service.AddPvz(addPvz, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Обновление данных о пункте выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="addPvz"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePvz(Guid id, [FromBody] AddPvzDto addPvz, CancellationToken cancellationToken)
    {
        var result = await service.UpdatePvz(id, addPvz, cancellationToken);
        return result.ToActionResult();
    }

    /// <summary>
    /// Удаление пункта выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePvz(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeletePvz(id, cancellationToken);
        return result.ToActionResult();
    }
}