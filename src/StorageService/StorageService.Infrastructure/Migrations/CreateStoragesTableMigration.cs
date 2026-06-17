using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520210300, "Create Storages Table")]
public class CreateStoragesTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("storages")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("address").AsString(255).NotNullable()
            .WithColumn("point_id").AsGuid().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("storages").Exists())
        {
            Delete.Table("storages");
        }
    }
}