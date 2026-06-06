namespace ProductService.Domain.Interfaces;

public interface IProductRepository
{
    Task Add(Product product, CancellationToken cancellationToken);
    
    Task<Product?> Get(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Product?>> GetProductsByFilter(ProductFilter filter, CancellationToken cancellationToken);

    Task Update(Guid id, Product product, CancellationToken cancellationToken);

    Task Delete(Guid id, CancellationToken cancellationToken);
}