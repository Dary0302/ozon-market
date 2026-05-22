using System.Data;
using FluentMigrator;

namespace OzonMarketOrder.Infrastructure.Migrations;

[Migration(202605222004, "Create Order Item Table")]
public class CreateOrderItemTable : Migration
{
    public override void Up()
    {
        Create.Table("order_items")
            .WithColumn("order_id").AsGuid().NotNullable()
                .ForeignKey("orders", "id").OnDelete(Rule.Cascade)
            .WithColumn("product_id").AsGuid().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable();
    }

    public override void Down()
    {
        if (Schema.Table("order_items").Exists())
        {
            Delete.Table("order_items");
        }
    }
}