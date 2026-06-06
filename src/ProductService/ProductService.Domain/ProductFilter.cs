namespace ProductService.Domain;

public record ProductFilter
{
    public string? Name { get; set; }
    public IReadOnlyCollection<ProductType>? Types { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? HasDiscount { get; set; }
    public decimal? MinDiscount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}