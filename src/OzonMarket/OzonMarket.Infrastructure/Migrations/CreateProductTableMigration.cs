using FluentMigrator;
using OzonMarket.Domain;

namespace OzonMarket.Infrastructure.Migrations;

[Migration(20260423132600, "Create Product Table")]
public class CreateProductTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("Products")
            .WithColumn(nameof(Product.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Product.Name)).AsString().NotNullable()
            .WithColumn(nameof(Product.Type)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("Products").Exists())
        {
            Delete.Table("Products");
        }
    }
}