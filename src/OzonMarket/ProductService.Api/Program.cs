using Core.Common;
using Core.Common.Migrations;
using ProductService.Api;
using ProductService.Infrastructure.Migrations;

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