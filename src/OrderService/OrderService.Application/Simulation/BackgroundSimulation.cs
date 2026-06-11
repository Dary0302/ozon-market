using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Simulation;

public class BackgroundSimulation : IBackgroundSimulation
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IHostApplicationLifetime appLifetime;

    public BackgroundSimulation(
        IServiceScopeFactory scopeFactory,
        IHostApplicationLifetime appLifetime)
    {
        this.scopeFactory = scopeFactory;
        this.appLifetime = appLifetime;
    }

    public void Enqueue(Func<IServiceProvider, CancellationToken, Task> job)
    {
        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            await job(scope.ServiceProvider, appLifetime.ApplicationStopping);
        });
    }
}