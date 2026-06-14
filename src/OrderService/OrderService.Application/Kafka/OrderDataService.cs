using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Contracts.Services;
using Core.Common.Kafka.Interfaces;
using FluentResults;
using OrderService.Application.Kafka.Interfaces;

namespace OrderService.Application.Kafka;

public class OrderDataService(
    IKafkaRequestClient<CheckStockRequest, KafkaResponse<CheckStockPayload>> stockClient,
    IKafkaRequestClient<GetDeliveryDateRequest, KafkaResponse<GetDeliveryDatePayload>> getDateClient,
    IKafkaRequestClient<GetOrderStorageRecordsRequest, KafkaResponse<GetOrderStorageRecordsPayload>> getStorageClient,
    IProductService productService)
    : IOrderDataService
{
    public async Task<Result<OrderData>> GetData(
        Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();

        var productQuantities = products.ToArray();
        var stockTask = stockClient.RequestAsync(new CheckStockRequest(correlationId, productQuantities, cancellationToken));
        var deliveryDateTask = getDateClient.RequestAsync(new GetDeliveryDateRequest(correlationId, pvzId, productQuantities, cancellationToken));
        var storageTask = getStorageClient.RequestAsync(new GetOrderStorageRecordsRequest(correlationId, pvzId, productQuantities, cancellationToken));
        var amountTask = productService.GetAmount(productQuantities, cancellationToken);

        await Task.WhenAll(stockTask, deliveryDateTask, amountTask, storageTask);

        if (!amountTask.Result.IsSuccess)
            return Result.Fail(amountTask.Result.Errors.Select(e => e.Message));
        if (!stockTask.Result.IsSuccess)
            return Result.Fail(stockTask.Result.Errors.Select(e => e.Message));
        if (!deliveryDateTask.Result.IsSuccess)
            return Result.Fail(deliveryDateTask.Result.Errors.Select(e => e.Message));
        if (!storageTask.Result.IsSuccess)
            return Result.Fail(storageTask.Result.Errors.Select(e => e.Message));

        return Result.Ok(new OrderData(
            amountTask.Result.Payload.Amount,
            deliveryDateTask.Result.Payload.Date,
            stockTask.Result.Payload.Items,
            storageTask.Result.Payload.Items));
    }

    public async Task<Result<IEnumerable<ProductPrice>>> GetPriceInfo(IEnumerable<ProductPriceRequest> requests, 
        CancellationToken cancellationToken)
    {
        var prices = await productService.GetPrices(requests, cancellationToken);
        /*if (!prices.IsSuccess)
            return Result.Fail(prices.Errors.Select(e => e.Message));*/
        return Result.Ok(prices);
    }
}