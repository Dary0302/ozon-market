using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Common.Migrations;

public static class MigrationRunner
{
    public static IHost RunMigrations<TMigrationMarker>(this IHost host)
    {
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                               ?? throw new NullReferenceException("DefaultConnection");

        var migrationsAssembly = typeof(TMigrationMarker).Assembly;
        
        var serviceContext = CreateService(connectionString, migrationsAssembly);
        using var scope = serviceContext.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        
        runner.Processor.Execute("CREATE SCHEMA IF NOT EXISTS public");
        runner.MigrateUp();

        return host;
    }

    private static IServiceProvider CreateService(string connectionString, Assembly migrationsAssembly)
        => new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(migrationsAssembly).For.Migrations()
                .ConfigureGlobalProcessorOptions(op => op.ProviderSwitches = "Force Quote=false"))
            .AddLogging(log => log.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
}