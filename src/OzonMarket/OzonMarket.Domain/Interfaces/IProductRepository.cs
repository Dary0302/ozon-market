namespace OzonMarket.Domain.Interfaces;

public interface IProductRepository
{
    Task Add(Product product);
    
    Task<Product> Get(Guid id);
}