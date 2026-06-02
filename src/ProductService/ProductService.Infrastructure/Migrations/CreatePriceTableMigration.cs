using System.Data;
using FluentMigrator;
using ProductService.Domain;

namespace ProductService.Infrastructure.Migrations;

[Migration(20209723132601, "Create prices Table")]
public class CreatePriceTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("prices")
            .WithColumn(nameof(Product.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Price.ProductId)).AsGuid()
                .NotNullable()
                .ForeignKey("products", "Id")
                .OnDelete(Rule.Cascade)
            .WithColumn(nameof(Price.Date)).AsDateTimeOffset().NotNullable()
            .WithColumn(nameof(Price.Cost)).AsDouble().NotNullable()
            .WithColumn(nameof(Price.Discount)).AsDecimal().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("prices").Exists())
        {
            Delete.Table("prices");
        }
    }
}