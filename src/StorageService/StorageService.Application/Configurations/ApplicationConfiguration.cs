using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Interfaces.Services;

namespace StorageService.Application.Configurations;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<IStorageService, Implementations.StorageService>()
            .AddScoped<IStoragePointService, Implementations.StoragePointService>()
            .AddScoped<IPvzService, Implementations.PvzService>()
            .AddScoped<IPvzPointService, Implementations.PvzPointService>()
            .AddScoped<IStoredProductService, Implementations.StoredProductService>();
        
        return services;
    }
}