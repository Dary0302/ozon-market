using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces;
using OrderService.Application.Implementations;
using OrderService.Application.Mocks;
using OrderService.Application.Simulation;

namespace OrderService.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderManagementService, OrderManagementService>();
        
        services.AddScoped<ISimulationService, SimulationService>();
        services.AddSingleton<IBackgroundSimulation, BackgroundSimulation>();
        
        return services;
    }
}