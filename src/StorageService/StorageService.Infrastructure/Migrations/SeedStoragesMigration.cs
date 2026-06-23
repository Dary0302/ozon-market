using FluentMigrator;

namespace StorageService.Infrastructure.Migrations;

[Migration(20260617002000, "Seed Storage Tables")]
public class SeedStoragesMigration : Migration
{
    private static readonly Guid Storage1 = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid Storage2 = new("a1000000-0000-0000-0000-000000000002");
    private static readonly Guid Storage3 = new("a1000000-0000-0000-0000-000000000003");
    private static readonly Guid Storage4 = new("a1000000-0000-0000-0000-000000000004");
    private static readonly Guid Storage5 = new("a1000000-0000-0000-0000-000000000005");
    
    private static readonly Guid StoragePoint1 = new("a2000000-0000-0000-0000-000000000001");
    private static readonly Guid StoragePoint2 = new("a2000000-0000-0000-0000-000000000002");
    private static readonly Guid StoragePoint3 = new("a2000000-0000-0000-0000-000000000003");
    private static readonly Guid StoragePoint4 = new("a2000000-0000-0000-0000-000000000004");
    private static readonly Guid StoragePoint5 = new("a2000000-0000-0000-0000-000000000005");
    
    private static readonly Guid Pvz1 = new("a3000000-0000-0000-0000-000000000001");
    private static readonly Guid Pvz2 = new("a3000000-0000-0000-0000-000000000002");
    private static readonly Guid Pvz3 = new("a3000000-0000-0000-0000-000000000003");
    private static readonly Guid Pvz4 = new("a3000000-0000-0000-0000-000000000004");
    private static readonly Guid Pvz5 = new("a3000000-0000-0000-0000-000000000005");
    
    private static readonly Guid PvzPoint1 = new("a4000000-0000-0000-0000-000000000001");
    private static readonly Guid PvzPoint2 = new("a4000000-0000-0000-0000-000000000002");
    private static readonly Guid PvzPoint3 = new("a4000000-0000-0000-0000-000000000003");
    private static readonly Guid PvzPoint4 = new("a4000000-0000-0000-0000-000000000004");
    private static readonly Guid PvzPoint5 = new("a4000000-0000-0000-0000-000000000005");
    
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
        Insert.IntoTable("storages").Rows(
            new { id = Storage1, address = "Москва, ул. Ленина, 1", point_id = StoragePoint1 },
            new { id = Storage2, address = "Санкт-Петербург, пр. Невский, 50", point_id = StoragePoint2 },
            new { id = Storage3, address = "Казань, ул. Баумана, 12", point_id = StoragePoint3 },
            new { id = Storage4, address = "Новосибирск, ул. Кирова, 7", point_id = StoragePoint4 },
            new { id = Storage5, address = "Курган, ул. Дзержинского, 24", point_id = StoragePoint5 }
        );
        
        Insert.IntoTable("storage_points").Rows(
            new { id = StoragePoint1, storage_id = Storage1, longitude = 37.6173, latitude = 55.7558 },
            new { id = StoragePoint2, storage_id = Storage2, longitude = 30.3351, latitude = 59.9343 },
            new { id = StoragePoint3, storage_id = Storage3, longitude = 49.1221, latitude = 55.7887 },
            new { id = StoragePoint4, storage_id = Storage4, longitude = 82.9346, latitude = 55.0084 },
            new { id = StoragePoint5, storage_id = Storage5, longitude = 22.1003, latitude = 45.7840 }
        );
        
        Insert.IntoTable("stored_products").Rows(
            new { product_id = Product1Id, storage_id = Storage1, quantity = 100 },
            new { product_id = Product2Id, storage_id = Storage2, quantity = 50  },
            new { product_id = Product3Id, storage_id = Storage3, quantity = 200 },
            new { product_id = Product4Id, storage_id = Storage4, quantity = 75  },
            new { product_id = Product5Id, storage_id = Storage5, quantity = 43  },
            new { product_id = Product6Id, storage_id = Storage1, quantity = 17 },
            new { product_id = Product7Id, storage_id = Storage2, quantity = 33  },
            new { product_id = Product8Id, storage_id = Storage3, quantity = 260 },
            new { product_id = Product9Id, storage_id = Storage4, quantity = 5  },
            new { product_id = Product10Id, storage_id = Storage5, quantity = 473  },
            new { product_id = Product11Id, storage_id = Storage1, quantity = 106 },
            new { product_id = Product12Id, storage_id = Storage2, quantity = 53  },
            new { product_id = Product13Id, storage_id = Storage3, quantity = 330 },
            new { product_id = Product14Id, storage_id = Storage4, quantity = 15  },
            new { product_id = Product15Id, storage_id = Storage5, quantity = 84  },
            new { product_id = Product16Id, storage_id = Storage1, quantity = 70 },
            new { product_id = Product17Id, storage_id = Storage2, quantity = 530  },
            new { product_id = Product18Id, storage_id = Storage3, quantity = 60 },
            new { product_id = Product19Id, storage_id = Storage4, quantity = 41  },
            new { product_id = Product20Id, storage_id = Storage5, quantity = 79  }
        );
        
        Insert.IntoTable("pvz").Rows(
            new { id = Pvz1, address = "Москва, ул. Тверская, 10", point_id = PvzPoint1 },
            new { id = Pvz2, address = "Санкт-Петербург, ул. Садовая, 3", point_id = PvzPoint2 },
            new { id = Pvz3, address = "Казань, ул. Петербургская, 8", point_id = PvzPoint3 },
            new { id = Pvz4, address = "Новосибирск, ул. Депутатская, 2", point_id = PvzPoint4 },
            new { id = Pvz5, address = "Курган, ул. Ленина, 2", point_id = PvzPoint5 }
        );
        
        Insert.IntoTable("pvz_points").Rows(
            new { id = PvzPoint1, pvz_id = Pvz1, longitude = 34.6155, latitude = 58.7650 },
            new { id = PvzPoint2, pvz_id = Pvz2, longitude = 25.3200, latitude = 47.9280 },
            new { id = PvzPoint3, pvz_id = Pvz3, longitude = 37.1100, latitude = 57.7950 },
            new { id = PvzPoint4, pvz_id = Pvz4, longitude = 78.9200, latitude = 78.0150 },
            new { id = PvzPoint5, pvz_id = Pvz5, longitude = 62.1003, latitude = 57.7840 }
        );
    }

    public override void Down()
    {
        Delete.FromTable("pvz_points").AllRows();
        Delete.FromTable("pvz").AllRows();
        Delete.FromTable("stored_products").AllRows();
        Delete.FromTable("storage_points").AllRows();
        Delete.FromTable("storages").AllRows();
    }
}