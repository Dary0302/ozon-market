using FluentMigrator;
using StorageService.Domain;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260520213700, "Create Pvz Table")]
public class CreatePvzTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("Pvz")
            .WithColumn(nameof(Pvz.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Pvz.Address)).AsString(255).NotNullable()
            .WithColumn(nameof(Pvz.PointId)).AsGuid().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("Pvz").Exists())
        {
            Delete.Table("Pvz");
        }
    }
}