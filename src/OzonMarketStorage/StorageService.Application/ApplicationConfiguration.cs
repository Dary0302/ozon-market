using Microsoft.Extensions.DependencyInjection;

namespace StorageService.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // services
            //TODO: Добавить реализации всех хендлеров и сервисов
            // .AddScoped<IAnimalService, AnimalService>()
            // .AddScoped<IEntityChangeHandler, HuntChangeHandler>()
        
        return services;
    }
}