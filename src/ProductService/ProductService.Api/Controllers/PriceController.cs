using Core.Common.Errors;
using Core.Common.Kafka.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Interfaces;
using ProductService.Domain;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/products/prices")]
public class PriceController(IPriceService service) : ControllerBase
{
    /// <summary>
    /// Получение суммы цен по списку продуктов
    /// </summary>
    /// <param name="productQuantities">Список id продуктов и их количества</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("amount")]
    public async Task<ActionResult<decimal>> GetAmount(
        [FromBody] IEnumerable<ProductQuantity> productQuantities,
        CancellationToken cancellationToken)
    {
        var productResult = await service.CalculateAmount(productQuantities, cancellationToken);
        return productResult.ToActionResult();
    }  
    
    /// <summary>
    /// Получение цены по id продукта
    /// </summary>
    /// <param name="productId">Id продукта</param>
    /// <param name="priceDate">Дата за которую нужна цена</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{product-id:guid}")]
    public async Task<ActionResult<decimal>> Get(
        [FromRoute(Name = "product-id")] Guid productId,
        [FromQuery(Name = "price-date")] DateTime? priceDate = null,
        CancellationToken cancellationToken = default)
    {
        var productResult = await service.GetActualPrice(productId, priceDate, cancellationToken);
        return productResult.ToActionResult();
    }
    
    /// <summary>
    /// Обновление цены продукта
    /// </summary>
    /// <param name="newPrice">Данные новой цены на продукт</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut]
    public async Task<ActionResult> Update(
        [FromBody] Price newPrice,
        CancellationToken cancellationToken)
    {
        var productResult = await service.SetPrice(newPrice, cancellationToken);
        return productResult.ToActionResult();
    }
}