using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Interfaces;
using FluentResults;
using OrderService.Application.Kafka.Interfaces;
using OrderService.Domain;

namespace OrderService.Application.Kafka;

public class OrderDataService(
    IKafkaRequestClient<CheckStockRequest, KafkaResponse<CheckStockPayload>> stockClient,
    IKafkaRequestClient<GetDeliveryDateRequest, KafkaResponse<GetDeliveryDatePayload>> getDateClient,
    IKafkaRequestClient<GetProductsStorageRequest, KafkaResponse<GetProductStoragePayload>> getStorageClient,
    IKafkaRequestClient<CalculateAmountRequest, KafkaResponse<CalculateAmountPayload>> calculateAmountClient,
    IKafkaRequestClient<GetProductsPriceRequest, KafkaResponse<GetProductsPricePayload>> getPricesClient)
    : IOrderDataService
{
    public async Task<Result<OrderData>> GetData(
        Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();

        var stockTask = stockClient.RequestAsync(new CheckStockRequest(correlationId, products));
        var deliveryDateTask = getDateClient.RequestAsync(new GetDeliveryDateRequest(correlationId, pvzId, products));
        var storageTask = getStorageClient.RequestAsync(new GetProductsStorageRequest(correlationId, products));
        var amountTask = calculateAmountClient.RequestAsync(new CalculateAmountRequest(correlationId, products));

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
        var correlationId = Guid.NewGuid();
        var prices = await getPricesClient.RequestAsync(
            new GetProductsPriceRequest(correlationId, requests));
        if (!prices.IsSuccess)
            return Result.Fail(prices.Errors.Select(e => e.Message));
        return Result.Ok(prices.Payload.ProductPrices);
    }
}