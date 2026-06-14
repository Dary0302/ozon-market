using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces;
using ProductService.Application.Services;

namespace ProductService.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<IProductManagementService, ProductManagementService>()
            .AddScoped<IPriceService, PriceService>()
            .AddScoped<IS3StorageService, S3StorageService>()
            .AddScoped<IPhotoService, PhotoService>();
        
        return services;
    }
}