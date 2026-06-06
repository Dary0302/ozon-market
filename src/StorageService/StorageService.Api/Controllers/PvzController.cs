using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Api.Dto;
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
    public async Task<ActionResult<IEnumerable<PvzDto>>> GetAllPvz()
    {
        var result = await service.GetAllPvz();
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
    public async Task<ActionResult<PvzDto>> GetPvz(Guid id)
    {
        var result = await service.GetPvz(id);
        return result.ToActionResult(pvz => pvz.ToHttp());
    }

    /// <summary>
    /// Добавление нового пункта выдачи заказов
    /// </summary>
    /// <param name="pvz"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> AddPvz([FromBody] Pvz pvz)
    {
        var result = await service.AddPvz(pvz);
        return result.ToActionResult();
    }

    /// <summary>
    /// Обновление данных о пункте выдачи заказов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pvz"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePvz(Guid id, [FromBody] Pvz pvz)
    {
        var result = await service.UpdatePvz(id, pvz);
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
    public async Task<ActionResult> DeletePvz(Guid id)
    {
        var result = await service.DeletePvz(id);
        return result.ToActionResult();
    }
}