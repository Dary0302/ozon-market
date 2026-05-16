using Destructurama;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Core.Common.LogLogic;

/// <summary>
/// Конфигурация Serilog
/// </summary>
public static class SerilogConfiguration
{
    private const string LogFormat = "{Timestamp:HH:mm:ss:ms} LEVEL:[{Level:u3}] TRACE:|{TraceId}| THREAD:{ThreadId} {Message}{NewLine}{Exception}";
    
    public static IServiceCollection AddLoggerServices(this IServiceCollection services)
    {
        return services
            .AddSingleton(Log.Logger);
    }

    public static LoggerConfiguration GetConfiguration(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .MinimumLevel.Is(LogEventLevel.Information)
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
            .Destructure.UsingAttributes()
            .WriteTo.Async(option =>
            {
                option.Console(LogEventLevel.Information, outputTemplate: LogFormat);
            });
    }
}