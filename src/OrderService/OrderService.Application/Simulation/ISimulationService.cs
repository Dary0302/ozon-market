using FluentResults;

namespace OrderService.Application.Simulation;

public interface ISimulationService
{
    public Task<Result> RunDeliverySimulation(Guid orderId, CancellationToken cancellationToken);
}