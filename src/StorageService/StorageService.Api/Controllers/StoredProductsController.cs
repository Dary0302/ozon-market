using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using StorageService.Api.Dto;
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
    /// <param name="storedProduct">Информация о продукте</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult> CreateStoredProduct([FromBody] StoredProduct storedProduct)
    {
        await service.AddStoredProduct(storedProduct);
        return Ok();
    }

    /// <summary>
    /// Получение всех продуктов с ненулевым количеством на складах 
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("stock")]
    public async Task<ActionResult<IEnumerable<ProductQuantityDto>>> GetStoredProductsInStock()
    {
        var result = await service.GetStoredProductsInStock();
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
    public async Task<ActionResult<DateTime>> GetDeliveryDate(Guid pvzId, [FromBody] List<ProductQuantity> orderedProducts)
    {
        var deliveryDate = await service.GetDeliveryDate(pvzId, orderedProducts);
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
    public async Task<ActionResult<List<DecreaseQuantityDto>>> GetStoredProductRecords(Guid pvzId, [FromBody] List<ProductQuantity> orderedProducts)
    {
        var result = await service.GetOrderStoragesRecords(pvzId, orderedProducts);
        return result.ToActionResult(records => records.Select(record => record.ToHttp()).ToList());
    }
}