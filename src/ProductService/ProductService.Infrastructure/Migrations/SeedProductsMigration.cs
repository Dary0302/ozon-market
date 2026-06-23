using FluentMigrator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Dto;
using ProductService.Application.Services;

namespace ProductService.Infrastructure.Migrations;

[Migration(20260423782800, "Seed products and prices")]
public class SeedProductsMigration(IConfiguration config) : Migration
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

    private readonly S3StorageService storageService = new S3StorageService(config);

    public override void Up()
    {
        var iPhone17PhotoId = GetPhotoId(PicturesForMigration.IPhone17);
        var samsungGalaxyS26PhotoId = GetPhotoId(PicturesForMigration.SamsungGalaxyS26);
        var googlePixel10PhotoId = GetPhotoId(PicturesForMigration.GooglePixel10);
        var iPadProM4PhotoId = GetPhotoId(PicturesForMigration.IPadProM4);
        var samsungGalaxyTabS10PhotoId = GetPhotoId(PicturesForMigration.SamsungGalaxyTabS10);
        var microsoftSurfacePro11PhotoId = GetPhotoId(PicturesForMigration.MicrosoftSurfacePro11);
        var airPodsPro3PhotoId = GetPhotoId(PicturesForMigration.AirPodsPro3);
        var sonyWh1000Xm6PhotoId = GetPhotoId(PicturesForMigration.SonyWH1000XM6);
        var macBookProM6PhotoId = GetPhotoId(PicturesForMigration.MacBookProM6);
        var dellXps15PhotoId = GetPhotoId(PicturesForMigration.DellXPS15);
        var lenovoThinkPadX1CarbonPhotoId = GetPhotoId(PicturesForMigration.LenovoThinkPadX1Carbon);
        var samsungQled65PhotoId = GetPhotoId(PicturesForMigration.SamsungQLED65);
        var lgoled55PhotoId = GetPhotoId(PicturesForMigration.LGOLED55);
        var appleWatchSeries11PhotoId = GetPhotoId(PicturesForMigration.AppleWatchSeries11);
        var samsungGalaxyWatch8PhotoId = GetPhotoId(PicturesForMigration.SamsungGalaxyWatch8);
        var playStation5PhotoId = GetPhotoId(PicturesForMigration.PlayStation5);
        var xboxSeriesX2PhotoId = GetPhotoId(PicturesForMigration.XboxSeriesX2);
        var sonyAlphaA7ViPhotoId = GetPhotoId(PicturesForMigration.SonyAlphaA7VI);
        var lgUltraWide34PhotoId = GetPhotoId(PicturesForMigration.LGUltraWide34);
        var samsungOdysseyG732PhotoId = GetPhotoId(PicturesForMigration.SamsungOdysseyG732);

        Insert.IntoTable("products")
            .InSchema("public")
            .Row(new { 
                id = Product1Id,  
                name = "iPhone 17",              
                description = "Apple smartphone",               
                type = 1, 
                photo_id = iPhone17PhotoId })
            .Row(new {
                id = Product2Id,  
                name = "Samsung Galaxy S26",      
                description = "Samsung smartphone",           
                type = 1, 
                photo_id = samsungGalaxyS26PhotoId })
            .Row(new { 
                id = Product3Id,  
                name = "Google Pixel 10",        
                description = "Google smartphone",            
                type = 1, 
                photo_id = googlePixel10PhotoId })
            .Row(new {
                id = Product4Id,  
                name = "iPad Pro M4",       
                description = "Apple tablet",             
                type = 2, photo_id = iPadProM4PhotoId })
            .Row(new { 
                id = Product5Id,  
                name = "Samsung Galaxy Tab S10",   
                description = "Samsung tablet",     
                type = 2, photo_id = samsungGalaxyTabS10PhotoId })
            .Row(new { 
                id = Product6Id, 
                name = "Microsoft Surface Pro 11",   
                description = "Microsoft tablet",        
                type = 2,
                photo_id = microsoftSurfacePro11PhotoId })
            .Row(new { 
                id = Product7Id,  
                name = "AirPods Pro 3",      
                description = "Apple wireless headphones",      
                type = 3, photo_id = airPodsPro3PhotoId })
            .Row(new { 
                id = Product8Id,  
                name = "Sony WH-1000XM6",          
                description = "Sony noise-cancelling headphones",
                type = 3, 
                photo_id = sonyWh1000Xm6PhotoId })
            .Row(new { 
                id = Product9Id,  
                name = "MacBook Pro M6",     
                description = "Apple laptop",             
                type = 4, 
                photo_id = macBookProM6PhotoId })
            .Row(new { 
                id = Product10Id, 
                name = "Dell XPS 15",               
                description = "Dell laptop",      
                type = 4, 
                photo_id = dellXps15PhotoId })
            .Row(new { 
                id = Product11Id, 
                name = "Lenovo ThinkPad X1 Carbon",  
                description = "Lenovo business laptop",       
                type = 4,
                photo_id = lenovoThinkPadX1CarbonPhotoId })
            .Row(new { 
                id = Product12Id, 
                name = "Samsung QLED 65",         
                description = "Samsung 4K QLED TV",      
                type = 5, 
                photo_id = samsungQled65PhotoId })
            .Row(new {
                id = Product13Id, 
                name = "LG OLED 55",          
                description = "LG 4K OLED TV",           
                type = 5, 
                photo_id = lgoled55PhotoId })
            .Row(new { 
                id = Product14Id, 
                name = "Apple Watch Series 11",      
                description = "Apple smartwatch",           
                type = 6, 
                photo_id = appleWatchSeries11PhotoId })
            .Row(new { 
                id = Product15Id, 
                name = "Samsung Galaxy Watch 8",    
                description = "Samsung smartwatch",      
                type = 6, 
                photo_id = samsungGalaxyWatch8PhotoId })
            .Row(new { 
                id = Product16Id, 
                name = "PlayStation 5",          
                description = "Sony gaming console",      
                type = 7, 
                photo_id = playStation5PhotoId })
            .Row(new { 
                id = Product17Id, 
                name = "Xbox Series X2",             
                description = "Microsoft gaming console",       
                type = 7, 
                photo_id = xboxSeriesX2PhotoId })
            .Row(new { 
                id = Product18Id,
                name = "Sony Alpha A7 VI",          
                description = "Sony mirrorless camera",  
                type = 8, 
                photo_id = sonyAlphaA7ViPhotoId })
            .Row(new { 
                id = Product19Id, 
                name = "LG UltraWide 34",         
                description = "LG ultrawide monitor",    
                type = 9,
                photo_id = lgUltraWide34PhotoId })
            .Row(new { 
                id = Product20Id, 
                name = "Samsung Odyssey G7 32",   
                description = "Samsung gaming monitor",        
                type = 9, 
                photo_id = samsungOdysseyG732PhotoId });

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

    private Guid GetPhotoId(string picture)
    {
        var bytes = Convert.FromBase64String(picture);
        var stream = new MemoryStream(bytes);
        return storageService.SaveImageAsync(stream, CancellationToken.None).Result.Value;
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