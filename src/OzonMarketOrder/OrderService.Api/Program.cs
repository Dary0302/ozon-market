using Core.Common;
using OrderService.Infrastructure.Migrations;
using OrderService.Api;
using Core.Common;
using Core.Common.Migrations;

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