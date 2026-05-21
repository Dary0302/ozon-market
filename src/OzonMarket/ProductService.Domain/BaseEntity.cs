namespace ProductService.Domain;

public record BaseEntity
{
    public Guid Id { get; private init; }
}