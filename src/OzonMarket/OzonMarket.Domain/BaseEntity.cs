using System.ComponentModel.DataAnnotations.Schema;

namespace OzonMarket.Domain;

public record BaseEntity
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; private init; }
}