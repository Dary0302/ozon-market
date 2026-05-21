using FluentMigrator;
using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Infrastructure.Migrations;

[Migration(20260520211300, "Create StoragePoints Table")]
public class CreateStoragePointsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("StoragePoints")
            .WithColumn(nameof(StoragePoint.PointId)).AsGuid().PrimaryKey()
            .WithColumn(nameof(StoragePoint.StorageId)).AsGuid().NotNullable().ForeignKey("Storages", "Id")
            .WithColumn(nameof(StoragePoint.Longitude)).AsDouble().NotNullable()
            .WithColumn(nameof(StoragePoint.Latitude)).AsDouble().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("StoragePoints").Exists())
        {
            Delete.Table("StoragePoints");
        }
    }
}