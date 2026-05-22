using System.ComponentModel.DataAnnotations.Schema;

namespace OzonMarketOrder.Domain;

public class BaseEntity
{
    public Guid Id { get; init; }
}