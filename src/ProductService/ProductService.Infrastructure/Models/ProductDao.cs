using ProductService.Domain;

namespace ProductService.Infrastructure.Models;

public record ProductDao(string Name, string Description, ProductType Type) : BaseEntityDao;