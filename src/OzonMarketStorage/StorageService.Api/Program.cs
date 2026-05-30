using StorageService.Api;
using Core.Common.Migrations;
using StorageService.Infrastructure.Migrations;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder =>
    {
        builder.UseStartup<Startup>();
    })
    .Build()
    .RunMigrations<MigrationMarker>()
    .Run();