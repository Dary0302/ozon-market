namespace ProductService.Domain.Interfaces;

public interface IProductRepository
{
    Task Add(Product product);
    
    Task<Product?> Get(Guid id);

    Task<IReadOnlyCollection<Product?>> GetProductsByFilter(ProductFilter filter);

    Task Update(Guid id, Product product);

    Task Delete(Guid id);
}