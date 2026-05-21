using FluentMigrator;
using ProductService.Domain;

namespace ProductService.Infrastructure.Migrations;

[Migration(20209723132601, "Create Price Table")]
public class CreatePriceTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("Prices")
            .WithColumn(nameof(Price.Id)).AsGuid().ForeignKey()
            .WithColumn(nameof(Price.Data)).AsDateTimeOffset().NotNullable()
            .WithColumn(nameof(Price.Cost)).AsDouble().NotNullable()
            .WithColumn(nameof(Price.Discount)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("Prices").Exists())
        {
            Delete.Table("Prices");
        }
    }
}