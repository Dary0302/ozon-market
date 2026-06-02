namespace ProductService.Domain;

public record ProductFilter(
    string? Name,
    IReadOnlyCollection<ProductType>? Types,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? HasDiscount,
    decimal? MinDiscount,
    decimal? MaxDiscount,
    int Page = 1,
    int PageSize = 20);