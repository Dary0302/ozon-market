using System.ComponentModel.DataAnnotations.Schema;

namespace OzonMarket.Domain;

public class BaseEntity
{
    [Column(TypeName = "uuid")]
    public Guid Id { get; private init; }
}