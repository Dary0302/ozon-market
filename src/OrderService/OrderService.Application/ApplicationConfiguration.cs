using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Application.Implementations;

namespace OrderService.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IOrderService, Implementations.OrderService>();
        
        return services;
    }
}