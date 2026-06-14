namespace ProductService.Domain.Interfaces;

public interface IPriceRepository
{
    Task SetPrice(Price price, CancellationToken cancellationToken);
    
    Task<Price?> GetPrice(Guid productId, DateTime? priceDate, CancellationToken cancellationToken);
    
    Task<IEnumerable<Price?>> GetPrices(List<Guid> productIds, CancellationToken cancellationToken);
}