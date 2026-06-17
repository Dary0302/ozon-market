using FluentMigrator;

namespace ProductService.Infrastructure.Migrations;

[Migration(20260423782800, "Seed products and prices")]
public class SeedProductsMigration : Migration
{
    private static readonly Guid Product1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Product2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Product3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid Product4Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid Product5Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public override void Up()
    {
        Insert.IntoTable("products")
            .InSchema("public")
            .Row(new
            {
                id = Product1Id,
                name = "iPhone 17",
                description = "Apple smartphone",
                type = 1,
                photo_id = (Guid?)null
            })
            .Row(new
            {
                id = Product2Id,
                name = "Samsung Galaxy S26",
                description = "Samsung smartphone",
                type = 1,
                photo_id = (Guid?)null
            })
            .Row(new
            {
                id = Product3Id,
                name = "MacBook Pro M6",
                description = "Apple laptop",
                type = 2,
                photo_id = (Guid?)null
            })
            .Row(new
            {
                id = Product4Id,
                name = "Dell XPS 15",
                description = "Dell laptop",
                type = 2,
                photo_id = (Guid?)null
            })
            .Row(new
            {
                id = Product5Id,
                name = "AirPods Pro 3",
                description = "Apple wireless headphones",
                type = 3,
                photo_id = (Guid?)null
            });

        Insert.IntoTable("prices")
            .InSchema("public")
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                product_id = Product1Id,
                date = DateTimeOffset.UtcNow,
                cost = 1299.99,
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                product_id = Product2Id,
                date = DateTimeOffset.UtcNow,
                cost = 1199.99,
                discount = 0.05m
            })
            .Row(new
            {
                id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                product_id = Product3Id,
                date = DateTimeOffset.UtcNow,
                cost = 2499.99,
                discount = 0.15m
            })
            .Row(new
            {
                id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                product_id = Product4Id,
                date = DateTimeOffset.UtcNow,
                cost = 1899.99,
                discount = 0.08m
            })
            .Row(new
            {
                id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                product_id = Product5Id,
                date = DateTimeOffset.UtcNow,
                cost = 299.99,
                discount = 0.20m
            });
    }

    public override void Down()
    {
        Delete.FromTable("prices")
            .Row(new { id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") })
            .Row(new { id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc") })
            .Row(new { id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd") })
            .Row(new { id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee") });

        Delete.FromTable("products")
            .Row(new { id = Product1Id })
            .Row(new { id = Product2Id })
            .Row(new { id = Product3Id })
            .Row(new { id = Product4Id })
            .Row(new { id = Product5Id });
    }
}