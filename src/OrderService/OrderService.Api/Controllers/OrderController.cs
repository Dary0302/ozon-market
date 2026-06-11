using Core.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Dto;
using OrderService.Api.Mappers;
using OrderService.Application.Interfaces;
using OrderService.Application.Mocks;
using OrderService.Application.Simulation;
using OrderService.Domain;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderManagementService service, IBackgroundSimulation simulation) : ControllerBase
{
    /// <summary>
    /// Создание заказа
    /// </summary>
    /// <param name="request">id пвз, посчитанная на клиенте сумма и список позиций из id и количества</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        var result = await service.Create(request.PvzId, request.ClientAmount, request.Products, cancellationToken);
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
    public async Task<ActionResult<OrderResponseDto>> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetById(id, cancellationToken);
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
    public async Task<ActionResult<Guid>> PayOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStatus(id, Status.Paid, cancellationToken);
        
        //Симуляция доставки
        if (result.IsSuccess)
        {
            simulation.Enqueue((serviceProvider, token) =>
            {
                var simulationService = serviceProvider.GetRequiredService<ISimulationService>();
                return simulationService.RunDeliverySimulation(id, token);
            });
        }
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Отмена заказа
    /// </summary>
    /// <param name="id">id заказа</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPatch("{id:guid}/cancel")]
    public async Task<ActionResult<Guid>> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.Cancel(id, cancellationToken);
        return result.ToActionResult();
    }
    
    /// <summary>
    /// Смена статуса заказа
    /// </summary>
    /// <param name="id">id заказа, новый статус</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<Guid>> ChangeOrderStatus(Guid id, 
        [FromBody] StatusDto newStatus, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStatus(id, newStatus.ToHttp(), cancellationToken);
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
    public async Task<ActionResult<OrderInfoResponseDto>> GetOrderInfo(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetInfoById(id, cancellationToken);
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
    public async Task<ActionResult<PagedResponseDto<OrderResponseDto>>> GetAllOrders([FromQuery]PagedRequestDto request, 
        CancellationToken cancellationToken)
    {
        var result = await service.GetAll(request.PageNumber, request.PageSize, cancellationToken);
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
    public async Task<ActionResult<PagedResponseDto<OrderInfoResponseDto>>> GetAllOrdersInfo([FromQuery]PagedRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAllInfo(request.PageNumber, request.PageSize, cancellationToken);
        return result.ToActionResult(orders => orders.ToHttp());
    }
}