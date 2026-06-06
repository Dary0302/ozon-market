using FluentMigrator;
using ProductService.Domain;

namespace ProductService.Infrastructure.Migrations;

[Migration(20209723132600, "Create products Table")]
public class CreateProductTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("products")
            .InSchema("public")
            .WithColumn(nameof(Product.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Product.Name)).AsString().NotNullable()
            .WithColumn(nameof(Product.Description)).AsString().NotNullable()
            .WithColumn(nameof(Product.Type)).AsInt32().NotNullable()
            .WithColumn(nameof(Product.PhotoId)).AsGuid();
    }

    public override void Down()
    {
        if (Schema.Table("products").Exists())
        {
            Delete.Table("products");
        }
    }
}