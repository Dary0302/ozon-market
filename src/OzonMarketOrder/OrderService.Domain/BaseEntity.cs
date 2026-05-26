using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Domain;

public record BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}