using FluentMigrator;
using OrderService.Domain;

namespace OrderService.Infrastructure.Migrations;

[Migration(202605221954, "Create Order Table")]
public class CreateOrderTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("orders")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("pvz_id").AsGuid().NotNullable()
            .WithColumn("created_on").AsDateTime().NotNullable()
            .WithColumn("delivery_date").AsDateTime().NotNullable()
            .WithColumn("status").AsInt32().NotNullable()
            .WithColumn("amount").AsDecimal(12, 2).NotNullable();
    }
    
    public override void Down()
    {
        if (Schema.Table("orders").Exists())
        {
            Delete.Table("orders");
        }
    }
}