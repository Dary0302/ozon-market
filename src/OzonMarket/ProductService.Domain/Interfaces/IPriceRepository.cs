namespace ProductService.Domain.Interfaces;

public interface IPriceRepository
{
    Task Add(Price price);
    
    Task<Price> Get(Guid id);
}