using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520214500, "Create PvzPoints Table")]
public class CreatePvzPointsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("pvz_points")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("pvz_id").AsGuid().NotNullable().ForeignKey("pvz", "id")
            .WithColumn("longitude").AsDouble().NotNullable()
            .WithColumn("latitude").AsDouble().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("pvz_points").Exists())
        {
            Delete.Table("pvz_points");
        }
    }
}