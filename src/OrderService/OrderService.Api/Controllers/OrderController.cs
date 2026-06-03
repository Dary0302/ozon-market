using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Dto;
using OrderService.Api.Mappers;
using OrderService.Application.Interfaces;
using OrderService.Domain;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderManagementService service) : Controller
{
    /// <summary>
    /// Создание заказа
    /// </summary>
    /// <param name="request">id пвз, посчитанная на клиенте сумма и список позиций из id и количества</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost("createOrder")]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var result = await service.Create(request.PvzId, request.ClientAmount, request.Products);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Получение данных о заказе
    /// </summary>
    /// <param name="id">id заказа</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetOrderResponseDto>> GetOrder(Guid id)
    {
        var result = await service.GetById(id);
        return result.ToActionResult(orders => orders.ToHttp());
    }
    
    /// <summary>
    /// Оплата заказа
    /// </summary>
    /// <param name="id">id заказа</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPatch("{id:guid}/pay")]
    public async Task<ActionResult<Guid>> PayOrder(Guid id)
    {
        var result = await service.UpdateStatus(id, Status.Paid);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Получение информации о заказе вместе с информацией о позициях
    /// </summary>
    /// <param name="id">id заказа</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<GetOrderInfoResponseDto>> GetOrderInfo(Guid id)
    {
        var result = await service.GetInfoById(id);
        return result.ToActionResult(orders => orders.ToHttp());
    }
    
    /// <summary>
    /// Получение информации о всех заказах
    /// </summary>
    /// <param name="request">Номер страницы и размер страницы для пагинации</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet]
    public async Task<ActionResult<PagedResponseDto<Order>>> GetAllOrders([FromQuery]PagedRequestDto request)
    {
        var result = await service.GetAll(request.PageNumber, request.PageSize);
        return result.ToActionResult(orders => orders.ToHttp());
    }
    
    /// <summary>
    /// Получение информации о всех заказах вместе с позициями
    /// </summary>
    /// <param name="request">Номер страницы и размер страницы для пагинации</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpGet("details")]
    public async Task<ActionResult<PagedResponseDto<OrderInfo>>> GetAllOrdersInfo([FromQuery]PagedRequestDto request)
    {
        var result = await service.GetAllInfo(request.PageNumber, request.PageSize);
        return result.ToActionResult(orders => orders.ToHttp());
    }
}