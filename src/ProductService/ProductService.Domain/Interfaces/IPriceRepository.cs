namespace ProductService.Domain.Interfaces;

public interface IPriceRepository
{
    Task<bool> SetPrice(Price price);
    
    Task<Price?> GetPrice(Guid productId);
    
    Task<List<Price>> GetPrices(List<Guid> productIds);

    Task SetDiscount(Guid productId, decimal discountPercent);
}