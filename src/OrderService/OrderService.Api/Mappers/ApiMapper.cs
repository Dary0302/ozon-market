using OrderService.Api.Dto;
using OrderService.Domain;
using OrderService.Application.Models;

namespace OrderService.Api.Mappers;

public static class ApiMapper
{
    public static OrderResponseDto ToHttp(this Order response)
    {
        return new OrderResponseDto
        {
            Id = response.Id,
            PvzId = response.PvzId,
            Status = response.Status.ToString(),
            DeliveryDate = response.DeliveryDate,
            CreatedOn = response.CreatedOn,
            Amount = response.Amount
        };
    }
    
    public static OrderItemResponseDto ToHttp(this OrderItemWithPrice response)
    {
        return new OrderItemResponseDto
        {
            ProductId = response.ProductId,
            Quantity = response.Quantity,
            Price = response.Price
        };
    }
    
    public static OrderInfoResponseDto ToHttp(this OrderInfoWithPrice response)
    {
        var items = response.OrderItems.Select(item => item.ToHttp());
        
        return new OrderInfoResponseDto
        {
            Id = response.Order.Id,
            PvzId = response.Order.PvzId,
            Status = response.Order.Status.ToString(),
            DeliveryDate = response.Order.DeliveryDate,
            Amount = response.Order.Amount,
            Products = items
        };
    }

    public static PagedResponseDto<OrderResponseDto> ToHttp(this PagedResult<Order> response)
    {
        return new PagedResponseDto<OrderResponseDto>(
            response.Items
                .Select(order => order.ToHttp()), 
            response.TotalCount);
    }
    
    public static PagedResponseDto<OrderInfoResponseDto> ToHttp(this PagedResult<OrderInfoWithPrice> response)
    {
        return new PagedResponseDto<OrderInfoResponseDto>(
            response.Items
                .Select(item => item.ToHttp()), 
            response.TotalCount);
    }

    public static Status ToHttp(this StatusDto status)
    {
        return (Status)status.Status;
    }
}