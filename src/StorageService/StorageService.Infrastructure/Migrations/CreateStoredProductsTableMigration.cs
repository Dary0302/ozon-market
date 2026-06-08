using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520211700, "Create StoredProducts Table")]
public class CreateStoredProductsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("stored_products")
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("storage_id").AsGuid().NotNullable().ForeignKey("storages", "id")
            .WithColumn("quantity").AsInt32().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("stored_products").Exists())
        {
            Delete.Table("stored_products");
        }
    }
}