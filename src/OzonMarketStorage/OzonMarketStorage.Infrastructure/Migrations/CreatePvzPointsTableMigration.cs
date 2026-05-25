using FluentMigrator;
using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Infrastructure.Migrations;

[Migration(20260520214500, "Create PvzPoints Table")]
public class CreatePvzPointsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("PvzPoints")
            .WithColumn(nameof(PvzPoint.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(PvzPoint.PvzId)).AsGuid().NotNullable().ForeignKey("Pvz", "Id")
            .WithColumn(nameof(PvzPoint.Longitude)).AsDouble().NotNullable()
            .WithColumn(nameof(PvzPoint.Latitude)).AsDouble().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("PvzPoints").Exists())
        {
            Delete.Table("PvzPoints");
        }
    }
}