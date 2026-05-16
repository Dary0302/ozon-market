using Core.Common.HttpLogic;
using Core.Common.LogLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Core.Common;

public static class CoreSetup
{
    public static IServiceCollection AddCore(this IServiceCollection services, IHostBuilder hostBuilder)
    {
        services
            .AddLoggerServices()
            .AddHttpRequestService();
        
        hostBuilder.UseSerilog(
            (builderContext, logConfiguration) => logConfiguration.GetConfiguration(),
            preserveStaticLogger: true);
        
        return services;
    } 
}