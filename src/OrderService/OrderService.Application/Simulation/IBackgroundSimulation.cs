namespace OrderService.Application.Simulation;

public interface IBackgroundSimulation
{
    void Enqueue(Func<IServiceProvider, CancellationToken, Task> job);
}