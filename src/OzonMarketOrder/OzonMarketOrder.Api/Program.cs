using Core.Common;
using OzonMarketOrder.Infrastructure.Migrations;
using OzonMarketOrder.Api;
// using Core.Common.Migrations;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());

hostBuilder.ConfigureServices(services => services.AddCore(hostBuilder));

hostBuilder
    .Build()
    // .RunMigration<MigrationMarker>()
    .Run();