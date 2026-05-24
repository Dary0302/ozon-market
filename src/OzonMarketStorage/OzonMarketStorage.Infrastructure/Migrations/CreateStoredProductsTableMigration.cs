using FluentMigrator;
using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Infrastructure.Migrations;

[Migration(20260520211700, "Create StoredProducts Table")]
public class CreateStoredProductsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("StoredProducts")
            .WithColumn(nameof(StoredProduct.Id)).AsGuid().NotNullable()
            .WithColumn(nameof(StoredProduct.StorageId)).AsGuid().NotNullable().ForeignKey("Storages", "Id")
            .WithColumn(nameof(StoredProduct.Quantity)).AsInt32().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("StoredProducts").Exists())
        {
            Delete.Table("StoredProducts");
        }
    }
}