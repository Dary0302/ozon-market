using FluentMigrator;
using ProductService.Domain;

namespace ProductService.Infrastructure.Migrations;

[Migration(20260423782600, "Create Product Table")]
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