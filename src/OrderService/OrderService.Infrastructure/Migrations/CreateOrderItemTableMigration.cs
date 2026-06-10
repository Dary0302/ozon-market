using System.Data;
using FluentMigrator;
using OrderService.Domain;

namespace OrderService.Infrastructure.Migrations;

[Migration(202605222004, "Create Order Item Table")]
public class CreateOrderItemTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("order_items")
            .WithColumn("order_id").AsGuid().NotNullable()
                .ForeignKey("orders", "id").OnDelete(Rule.Cascade)
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable();
        
        Create.PrimaryKey("PK_order_items")
            .OnTable("order_items")
            .Columns("order_id", "product_id");
    }

    public override void Down()
    {
        if (Schema.Table("order_items").Exists())
        {
            Delete.Table("order_items");
        }
    }
}