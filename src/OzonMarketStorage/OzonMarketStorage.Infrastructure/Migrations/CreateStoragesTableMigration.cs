using FluentMigrator;
using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Infrastructure.Migrations;

[Migration(20260520210300, "Create Storages Table")]
public class CreateStoragesTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("Storages")
            .WithColumn(nameof(Storage.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Storage.Address)).AsString(255).NotNullable()
            .WithColumn(nameof(Storage.PointId)).AsString(255).NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("Storages").Exists())
        {
            Delete.Table("Storages");
        }
    }
}