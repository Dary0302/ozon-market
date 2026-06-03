using OrderService.Api.Dto;
using OrderService.Domain;
using OrderService.Application.Models;

namespace OrderService.Api.Mappers;

public static class ApiMapper
{
    public static GetOrderResponseDto ToHttp(this Order response)
    {
        return new GetOrderResponseDto
        {
            Id = response.Id,
            PvzId = response.PvzId,
            Status = response.Status.ToString(),
            DeliveryDate = response.DeliveryDate,
            CreatedOn = response.CreatedOn,
            Amount = response.Amount
        };
    }
    
    public static GetOrderInfoResponseDto ToHttp(this OrderInfo response)
    {
        return new GetOrderInfoResponseDto
        {
            Id = response.Order.Id,
            PvzId = response.Order.PvzId,
            Status = response.Order.Status.ToString(),
            DeliveryDate = response.Order.DeliveryDate,
            Amount = response.Order.Amount,
            Products = response.OrderItems
        };
    }

    public static PagedResponseDto<Order> ToHttp(this PagedResult<Order> response)
    {
        return new PagedResponseDto<Order>(response.Items, response.TotalCount);
    }
    
    public static PagedResponseDto<OrderInfo> ToHttp(this PagedResult<OrderInfo> response)
    {
        return new PagedResponseDto<OrderInfo>(response.Items, response.TotalCount);
    }
}