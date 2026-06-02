namespace ProductService.Domain.Interfaces;

public interface IPriceRepository
{
    Task SetPrice(Price price);
    
    Task<Price?> GetPrice(Guid productId);
    
    Task<IEnumerable<Price?>> GetPrices(List<Guid> productIds);
}