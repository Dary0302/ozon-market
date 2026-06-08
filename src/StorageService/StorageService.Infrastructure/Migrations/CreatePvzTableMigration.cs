using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520213700, "Create Pvz Table")]
public class CreatePvzTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("pvz")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("address").AsString(255).NotNullable()
            .WithColumn("point_id").AsGuid().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("pvz").Exists())
        {
            Delete.Table("pvz");
        }
    }
}