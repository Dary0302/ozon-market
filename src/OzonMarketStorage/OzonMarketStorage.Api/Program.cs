using OzonMarketStorage.Api;
using Core.Common.Migrations;
using Core.Common;
using OzonMarketStorage.Infrastructure.Migrations;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build()
    .RunMigrations<MigrationMarker>()
    .Run();