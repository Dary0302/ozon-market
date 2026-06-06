using FluentMigrator;
using OrderService.Domain;

namespace OrderService.Infrastructure.Migrations;

[Migration(202605221954, "Create Order Table")]
public class CreateOrderTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("orders")
            .WithColumn(nameof(Order.Id)).AsGuid().PrimaryKey()
            .WithColumn(nameof(Order.PvzId)).AsGuid().NotNullable()
            .WithColumn(nameof(Order.CreatedOn)).AsDateTime().NotNullable()
            .WithColumn(nameof(Order.DeliveryDate)).AsDateTime().NotNullable()
            .WithColumn(nameof(Order.Status)).AsInt32().NotNullable()
            .WithColumn(nameof(Order.Amount)).AsDecimal(12, 2).NotNullable();
    }
    
    public override void Down()
    {
        if (Schema.Table("orders").Exists())
        {
            Delete.Table("orders");
        }
    }
}