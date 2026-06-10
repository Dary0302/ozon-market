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
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("description").AsString().NotNullable()
            .WithColumn("type").AsInt32().NotNullable()
            .WithColumn("photo_id").AsGuid();
    }

    public override void Down()
    {
        if (Schema.Table("products").Exists())
        {
            Delete.Table("products");
        }
    }
}