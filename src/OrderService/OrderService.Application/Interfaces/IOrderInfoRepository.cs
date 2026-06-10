using OrderService.Application.Models;
using OrderService.Domain;

namespace OrderService.Application.Interfaces;

public interface IOrderInfoRepository
{
    Task<PagedResult<OrderInfo>> GetAll(int pageNumber, int pageSize, CancellationToken cancellationToken);
}