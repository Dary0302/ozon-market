using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Dto;
using StorageService.Api.Mappers;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Api.Controllers;

[ApiController]
[Route("api/stored-products")]
public class StoredProductsController(IStoredProductService service) : ControllerBase
{
    /// <summary>
    /// Добавление продукта на склад
    /// </summary>
    /// <param name="addStoredProduct">Информация о продукте</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult> CreateStoredProduct([FromBody] AddStoredProductDto addStoredProduct, CancellationToken cancellationToken)
    {
        await service.AddStoredProduct(addStoredProduct, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Получение всех продуктов с ненулевым количеством на складах 
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("stock")]
    public async Task<ActionResult<IEnumerable<ProductQuantityDto>>> GetStoredProductsInStock(CancellationToken cancellationToken)
    {
        var result = await service.GetStoredProductsInStock(cancellationToken);
        return result.ToActionResult(products => products.Select(product => product.ToHttp()));
    }

    /// <summary>
    /// Получение примерной даты доставки для заказа
    /// </summary>
    /// <param name="pvzId">Id пункта выдачи заказов</param>
    /// <param name="orderedProducts">Список заказанных товаров</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{pvzId:guid}/date")]
    public async Task<ActionResult<DateTime>> GetDeliveryDate(Guid pvzId, [FromBody] List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var deliveryDate = await service.GetDeliveryDate(pvzId, orderedProducts, cancellationToken);
        return deliveryDate.ToActionResult();
    }

    /// <summary>
    /// Получение записей об удалении товаров со склада
    /// </summary>
    /// <param name="pvzId">Id пункта выдачи заказов</param>
    /// <param name="orderedProducts">Список заказанных товаров</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{pvzId:guid}/records")]
    public async Task<ActionResult<List<DecreaseQuantityDto>>> GetStoredProductRecords(Guid pvzId, [FromBody] List<ProductQuantity> orderedProducts, CancellationToken cancellationToken)
    {
        var result = await service.GetOrderStoragesRecords(pvzId, orderedProducts, cancellationToken);
        return result.ToActionResult(records => records.Select(record => record.ToHttp()).ToList());
    }
}