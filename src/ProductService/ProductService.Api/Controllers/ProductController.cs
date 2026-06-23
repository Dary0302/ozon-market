using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using ProductService.Api.Mappers;
using ProductService.Application.Dto;
using ProductService.Application.Interfaces;
using ProductService.Domain;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController(IProductManagementService service) : ControllerBase
{
    /// <summary>
    /// Получение продукта по id
    /// </summary>
    /// <param name="productId">Id продукта</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{product-id:guid}")]
    public async Task<ActionResult<GetProductDto>> Get(
        [FromRoute(Name = "product-id")] Guid productId,
        CancellationToken cancellationToken)
    {
        var productResult = await service.GetProduct(productId, cancellationToken);
        return productResult.ToActionResult(product => product.ToHttp())!;
    }

    /// <summary>
    /// Получение списка продуктов по фильтру
    /// </summary>
    /// <param name="filter">Критерии фильтрации продуктов</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult<IEnumerable<GetProductDto?>>> GetProducts(
        [FromBody] ProductFilter filter,
        CancellationToken cancellationToken)
    {
        var productsResult = await service.GetProducts(filter, cancellationToken);
        return productsResult
            .ToActionResult(products => 
                products.Select(product => 
                    product.ToHttp()));
    }

    /// <summary>
    /// Добавление продукта
    /// </summary>
    /// <param name="product">Данные продукта</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost("add")]
    public async Task<ActionResult<Guid>> Add(
        [FromBody] CreateProductDto product,
        CancellationToken cancellationToken)
    {
        var addProductResult = await service.AddProduct(product, cancellationToken);
        return addProductResult.ToActionResult();
    }

    /// <summary>
    /// Обновление продукта по id
    /// </summary>
    /// <param name="productId">Id старого продукта</param>
    /// <param name="newProduct">Данные нового продукта</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{product-id:guid}")]
    public async Task<ActionResult> Update(
        [FromRoute(Name = "product-id")] Guid productId,
        [FromBody] CreateProductDto newProduct,
        CancellationToken cancellationToken)
    {
        var productResult = await service.UpdateProduct(productId, newProduct, cancellationToken);
        return productResult.ToActionResult();
    }

    /// <summary>
    /// Удаление продукта по id
    /// </summary>
    /// <param name="productId">Id продукта</param>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{product-id:guid}")]
    public async Task<ActionResult> Delete(
        [FromRoute(Name = "product-id")] Guid productId,
        CancellationToken cancellationToken)
    {
        var productResult = await service.DeleteProduct(productId, cancellationToken);
        return productResult.ToActionResult();
    }
}