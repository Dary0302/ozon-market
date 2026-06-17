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
    
    private static readonly Guid Product1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Product2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Product3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid Product4 = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid Product5 = Guid.Parse("55555555-5555-5555-5555-555555555555");

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
            new { product_id = Product1, storage_id = Storage1, quantity = 100 },
            new { product_id = Product2, storage_id = Storage2, quantity = 50  },
            new { product_id = Product3, storage_id = Storage3, quantity = 200 },
            new { product_id = Product4, storage_id = Storage4, quantity = 75  },
            new { product_id = Product5, storage_id = Storage5, quantity = 43  }
        );
        
        Insert.IntoTable("pvz").Rows(
            new { id = Pvz1, address = "Москва, ул. Тверская, 10", point_id = PvzPoint1 },
            new { id = Pvz2, address = "Санкт-Петербург, ул. Садовая, 3", point_id = PvzPoint2 },
            new { id = Pvz3, address = "Казань, ул. Петербургская, 8", point_id = PvzPoint3 },
            new { id = Pvz4, address = "Новосибирск, ул. Депутатская, 2", point_id = PvzPoint4 },
            new { id = Pvz5, address = "Курган, ул. Ленина, 2", point_id = PvzPoint5 }
        );
        
        Insert.IntoTable("pvz_points").Rows(
            new { id = PvzPoint1, pvz_id = Pvz1, longitude = 37.6155, latitude = 55.7650 },
            new { id = PvzPoint2, pvz_id = Pvz2, longitude = 30.3200, latitude = 59.9280 },
            new { id = PvzPoint3, pvz_id = Pvz3, longitude = 49.1100, latitude = 55.7950 },
            new { id = PvzPoint4, pvz_id = Pvz4, longitude = 82.9200, latitude = 55.0150 },
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