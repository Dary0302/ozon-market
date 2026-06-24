using FluentResults;
using OrderService.Application.Interfaces;
using OrderService.Domain;
using OrderService.Domain.Exseptions;

namespace OrderService.Application.Simulation;

public class SimulationService(IOrderRepository orderRepository, IOrderManagementService service) : ISimulationService
{
    public async Task<Result> RunDeliverySimulation(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetById(orderId, cancellationToken);
        if (order is null)
            return Result.Fail(OrderErrors.NotFound(orderId));

        var collectResult = await service.UpdateStatus(orderId, Status.InAssembly, cancellationToken);
        if (collectResult.IsFailed)
            return Result.Fail(collectResult.Errors);
        
        var assemblyDelay = TimeSpan.FromSeconds(10 + Random.Shared.Next(-2, 3));
        await Task.Delay(assemblyDelay, cancellationToken);
        
        var transferResult = await service.UpdateStatus(orderId, Status.TransferredForDelivery, cancellationToken);
        if (transferResult.IsFailed)
            return Result.Fail(transferResult.Errors);
        
        var deliveryDuration = order.DeliveryDate - order.CreatedOn;
        var scaledDelaySeconds = Math.Max(0, deliveryDuration.TotalSeconds / 7200);
        await Task.Delay(TimeSpan.FromSeconds(scaledDelaySeconds), cancellationToken);
        
        var completeResult = await service.UpdateStatus(orderId, Status.Delivered, cancellationToken);
        if (completeResult.IsFailed)
            return Result.Fail(completeResult.Errors);

        return Result.Ok();
    }
}