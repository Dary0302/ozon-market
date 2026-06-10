using ProductService.Domain;
using ProductService.Infrastructure.Models;

namespace ProductService.Infrastructure.Mappers;

public static class DaoMapper
{
    public static Product? ToDomain(this ProductDao? dao)
    {
        return dao is null
            ? null
            : new Product(dao.Name, dao.Description, dao.Type, dao.PhotoId) { Id = dao.Id };
    }

    public static Price? ToDomain(this PriceDao? dao)
    {
        return dao is null
            ? null
            : new Price(dao.ProductId, dao.Cost, dao.Discount) { Id = dao.Id, Date = dao.Date };
    }
}