using System.Data;
using FluentMigrator;
using ProductService.Domain;

namespace ProductService.Infrastructure.Migrations;

[Migration(20260423782700, "Create prices Table")]
public class CreatePriceTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("prices")
            .InSchema("public")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("product_id").AsGuid()
                .NotNullable()
                .ForeignKey("public", "products", "Id")
                .OnDelete(Rule.Cascade)
            .WithColumn("date").AsDateTimeOffset().NotNullable()
            .WithColumn("cost").AsDouble().NotNullable()
            .WithColumn("discount").AsDecimal().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("prices").Exists())
        {
            Delete.Table("prices");
        }
    }
}