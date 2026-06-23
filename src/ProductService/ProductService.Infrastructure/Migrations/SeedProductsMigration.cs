using FluentMigrator;

namespace ProductService.Infrastructure.Migrations;

[Migration(20260423782800, "Seed products and prices")]
public class SeedProductsMigration : Migration
{
    // Phones
    private static readonly Guid Product1Id  = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Product2Id  = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Product3Id  = Guid.Parse("33333333-3333-3333-3333-333333333333");

    // Tablets 
    private static readonly Guid Product4Id  = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid Product5Id  = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid Product6Id  = Guid.Parse("66666666-6666-6666-6666-666666666666");

    // Headphones 
    private static readonly Guid Product7Id  = Guid.Parse("77777777-7777-7777-7777-777777777777");
    private static readonly Guid Product8Id  = Guid.Parse("88888888-8888-8888-8888-888888888888");

    // Laptops
    private static readonly Guid Product9Id  = Guid.Parse("99999999-9999-9999-9999-999999999999");
    private static readonly Guid Product10Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    private static readonly Guid Product11Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");

    // TVs 
    private static readonly Guid Product12Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001");
    private static readonly Guid Product13Id = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002");

    // Smartwatches
    private static readonly Guid Product14Id = Guid.Parse("cccccccc-0000-0000-0000-000000000001");
    private static readonly Guid Product15Id = Guid.Parse("cccccccc-0000-0000-0000-000000000002");

    // Consoles
    private static readonly Guid Product16Id = Guid.Parse("dddddddd-0000-0000-0000-000000000001");
    private static readonly Guid Product17Id = Guid.Parse("dddddddd-0000-0000-0000-000000000002");

    // Cameras
    private static readonly Guid Product18Id = Guid.Parse("eeeeeeee-0000-0000-0000-000000000001");

    // Monitors
    private static readonly Guid Product19Id = Guid.Parse("ffffffff-0000-0000-0000-000000000001");
    private static readonly Guid Product20Id = Guid.Parse("ffffffff-0000-0000-0000-000000000002");

    public override void Up()
    {
        Insert.IntoTable("products")
            .InSchema("public")
            .Row(new { 
                id = Product1Id,  
                name = "iPhone 17",              
                description = "Apple smartphone",               
                type = 1, 
                photo_id = (Guid?)null })
            .Row(new {
                id = Product2Id,  
                name = "Samsung Galaxy S26",      
                description = "Samsung smartphone",           
                type = 1, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product3Id,  
                name = "Google Pixel 10",        
                description = "Google smartphone",            
                type = 1, 
                photo_id = (Guid?)null })
            .Row(new {
                id = Product4Id,  
                name = "iPad Pro M4",       
                description = "Apple tablet",             
                type = 2, photo_id = (Guid?)null })
            .Row(new { 
                id = Product5Id,  
                name = "Samsung Galaxy Tab S10",   
                description = "Samsung tablet",     
                type = 2, photo_id = (Guid?)null })
            .Row(new { 
                id = Product6Id, 
                name = "Microsoft Surface Pro 11",   
                description = "Microsoft tablet",        
                type = 2,
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product7Id,  
                name = "AirPods Pro 3",      
                description = "Apple wireless headphones",      
                type = 3, photo_id = (Guid?)null })
            .Row(new { 
                id = Product8Id,  
                name = "Sony WH-1000XM6",          
                description = "Sony noise-cancelling headphones",
                type = 3, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product9Id,  
                name = "MacBook Pro M6",     
                description = "Apple laptop",             
                type = 4, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product10Id, 
                name = "Dell XPS 15",               
                description = "Dell laptop",      
                type = 4, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product11Id, 
                name = "Lenovo ThinkPad X1 Carbon",  
                description = "Lenovo business laptop",       
                type = 4,
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product12Id, 
                name = "Samsung QLED 65",         
                description = "Samsung 4K QLED TV",      
                type = 5, 
                photo_id = (Guid?)null })
            .Row(new {
                id = Product13Id, 
                name = "LG OLED 55",          
                description = "LG 4K OLED TV",           
                type = 5, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product14Id, 
                name = "Apple Watch Series 11",      
                description = "Apple smartwatch",           
                type = 6, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product15Id, 
                name = "Samsung Galaxy Watch 8",    
                description = "Samsung smartwatch",      
                type = 6, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product16Id, 
                name = "PlayStation 5",          
                description = "Sony gaming console",      
                type = 7, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product17Id, 
                name = "Xbox Series X2",             
                description = "Microsoft gaming console",       
                type = 7, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product18Id,
                name = "Sony Alpha A7 VI",          
                description = "Sony mirrorless camera",  
                type = 8, 
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product19Id, 
                name = "LG UltraWide 34",         
                description = "LG ultrawide monitor",    
                type = 9,
                photo_id = (Guid?)null })
            .Row(new { 
                id = Product20Id, 
                name = "Samsung Odyssey G7 32",   
                description = "Samsung gaming monitor",        
                type = 9, 
                photo_id = (Guid?)null });

        Insert.IntoTable("prices")
            .InSchema("public")
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product1Id, 
                date = DateTimeOffset.UtcNow, 
                cost = 1299.99m, 
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                product_id = Product2Id, 
                date = DateTimeOffset.UtcNow,
                cost = 1199.99m, 
                discount = 0.05m
            })
            .Row(new
            {
                id = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product3Id, 
                date = DateTimeOffset.UtcNow,
                cost =  999.99m, 
                discount = 0.07m
            })
            .Row(new
            {
                id = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product4Id, 
                date = DateTimeOffset.UtcNow, 
                cost = 1099.99m, 
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product5Id,  
                date = DateTimeOffset.UtcNow, 
                cost =  849.99m, 
                discount = 0.08m
            })
            .Row(new
            {
                id = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product6Id, 
                date = DateTimeOffset.UtcNow,
                cost = 1299.99m, 
                discount = 0.12m
            })
            .Row(new
            {
                id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                product_id = Product7Id, 
                date = DateTimeOffset.UtcNow, 
                cost =  299.99m, 
                discount = 0.20m
            })
            .Row(new
            {
                id = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                product_id = Product8Id,  
                date = DateTimeOffset.UtcNow, 
                cost =  349.99m,
                discount = 0.15m
            })
            .Row(new
            {
                id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), 
                product_id = Product9Id,  
                date = DateTimeOffset.UtcNow, 
                cost = 2499.99m, 
                discount = 0.15m
            })
            .Row(new
            {
                id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                product_id = Product10Id,
                date = DateTimeOffset.UtcNow, 
                cost = 1899.99m, 
                discount = 0.08m
            })
            .Row(new
            {
                id = Guid.Parse("66666666-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product11Id, 
                date = DateTimeOffset.UtcNow, 
                cost = 1599.99m, 
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("77777777-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product12Id, 
                date = DateTimeOffset.UtcNow,
                cost = 1799.99m, 
                discount = 0.12m
            })
            .Row(new
            {
                id = Guid.Parse("88888888-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 
                product_id = Product13Id,
                date = DateTimeOffset.UtcNow, 
                cost = 1499.99m,
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("99999999-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                product_id = Product14Id,
                date = DateTimeOffset.UtcNow,
                cost =  499.99m,
                discount = 0.08m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"),
                product_id = Product15Id,
                date = DateTimeOffset.UtcNow, 
                cost =  349.99m,
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"), 
                product_id = Product16Id, 
                date = DateTimeOffset.UtcNow, 
                cost =  699.99m, 
                discount = 0.05m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"), 
                product_id = Product17Id, 
                date = DateTimeOffset.UtcNow, 
                cost =  649.99m, 
                discount = 0.05m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"),
                product_id = Product18Id, 
                date = DateTimeOffset.UtcNow, 
                cost = 2799.99m, 
                discount = 0.07m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"), 
                product_id = Product19Id,
                date = DateTimeOffset.UtcNow, 
                cost =  799.99m, 
                discount = 0.10m
            })
            .Row(new
            {
                id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006"), 
                product_id = Product20Id, 
                date = DateTimeOffset.UtcNow, 
                cost =  649.99m, 
                discount = 0.12m
            });
    }

    public override void Down()
    {
        Delete.FromTable("prices").InSchema("public")
            .Row(new { id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") })
            .Row(new { id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc") })
            .Row(new { id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd") })
            .Row(new { id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee") })
            .Row(new { id = Guid.Parse("11111111-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("22222222-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("33333333-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("44444444-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("55555555-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("66666666-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("77777777-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("88888888-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("99999999-aaaa-aaaa-aaaa-aaaaaaaaaaaa") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005") })
            .Row(new { id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006") });

        Delete.FromTable("products").InSchema("public")
            .Row(new { id = Product1Id })
            .Row(new { id = Product2Id })
            .Row(new { id = Product3Id })
            .Row(new { id = Product4Id })
            .Row(new { id = Product5Id })
            .Row(new { id = Product6Id })
            .Row(new { id = Product7Id })
            .Row(new { id = Product8Id })
            .Row(new { id = Product9Id })
            .Row(new { id = Product10Id })
            .Row(new { id = Product11Id })
            .Row(new { id = Product12Id })
            .Row(new { id = Product13Id })
            .Row(new { id = Product14Id })
            .Row(new { id = Product15Id })
            .Row(new { id = Product16Id })
            .Row(new { id = Product17Id })
            .Row(new { id = Product18Id })
            .Row(new { id = Product19Id })
            .Row(new { id = Product20Id });
    }
}