namespace ProductService.Domain.Interfaces;

public interface IProductRepository
{
    Task Add(Product product);
    
    Task<Product> Get(Guid id);
}