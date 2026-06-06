namespace ProductService.Domain;

public record BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}