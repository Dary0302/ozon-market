using FluentMigrator;

namespace OrderService.Infrastructure.Migrations;

[Migration(202605221954, "Create Order Table")]
public class CreateOrderTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("orders")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("pvz_id").AsGuid().NotNullable()
            .WithColumn("date").AsDateTime().NotNullable()
            .WithColumn("status").AsInt32().NotNullable()
            .WithColumn("amount").AsDouble().NotNullable();
    }
    
    public override void Down()
    {
        if (Schema.Table("orders").Exists())
        {
            Delete.Table("orders");
        }
    }
}