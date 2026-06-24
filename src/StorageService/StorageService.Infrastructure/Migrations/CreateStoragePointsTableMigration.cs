using System.Data;
using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520211300, "Create StoragePoints Table")]
public class CreateStoragePointsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("storage_points")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("storage_id").AsGuid().NotNullable()
                .ForeignKey("storages", "id").OnDelete(Rule.Cascade)
            .WithColumn("longitude").AsDouble().NotNullable()
            .WithColumn("latitude").AsDouble().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("storage_points").Exists())
        {
            Delete.Table("storage_points");
        }
    }
}