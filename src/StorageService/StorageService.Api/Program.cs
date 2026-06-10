using Core.Common;
using StorageService.Api;
using Core.Common.Migrations;
using StorageService.Infrastructure.Migrations;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddCore(Host.CreateDefaultBuilder(args));
    })
    .ConfigureWebHostDefaults(builder =>
        builder.UseStartup<Startup>())
    .Build();

host.RunMigrations<MigrationMarker>()
    .Run();