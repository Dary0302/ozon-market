using Core.Common;
using OrderService.Infrastructure.Migrations;
using OrderService.Api;
// using Core.Common.Migrations;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());

hostBuilder.ConfigureServices(services => services.AddCore(hostBuilder));

hostBuilder
    .Build()
    // .RunMigration<MigrationMarker>()
    .Run();