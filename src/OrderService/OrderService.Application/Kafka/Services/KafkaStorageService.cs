using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Contracts.Services;
using Core.Common.Kafka.Interfaces;
using Volo.Abp;

namespace OrderService.Application.Kafka.Services;

public class KafkaStorageService(IKafkaRpcClient rpc, IKafkaProducer producer) : IStorageService
{
    public async Task<IEnumerable<StockCheckResult>> CheckStock(IEnumerable<ProductQuantity> items, CancellationToken ct)
    {
        var response = 
            await rpc.RequestAsync<CheckStockRequest, KafkaResponse<CheckStockPayload>>(KafkaTopics
                .CheckStockRequests,
            new CheckStockRequest(Guid.NewGuid(),
                items),
            ct);

        if (!response.IsSuccess)
        {
            throw new BusinessException(response.Errors
                .Select(error => error.Message).ToString());
        }

        return response.Payload!.Items;
    }

    public async Task<DateTime> GetDeliveryDate(Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken ct)
    {
        var response = 
            await rpc.RequestAsync<GetDeliveryDateRequest, KafkaResponse<GetDeliveryDatePayload>>(KafkaTopics
                    .DeliveryDateRequests,
                new GetDeliveryDateRequest(Guid.NewGuid(),
                    pvzId,
                    products), 
                ct);

        if (!response.IsSuccess)
        {
            throw new BusinessException(response.Errors
                .Select(error => error.Message).ToString());
        }

        return response.Payload!.Date;
    }

    public async Task ReduceCountOfProducts(IEnumerable<DecreaseQuantity> items, CancellationToken ct)
    {
        await producer.ProduceAsync(
            KafkaTopics.ReduceStockCommand,
            new ReduceCountOfProductsCommand(items),
            ct);
    }

    public async Task<IEnumerable<DecreaseQuantity>> GetOrderStorageRecords(Guid pvzId, IEnumerable<ProductQuantity> products, CancellationToken ct)
    {
        var response = 
            await rpc.RequestAsync<GetOrderStorageRecordsRequest, KafkaResponse<GetOrderStorageRecordsPayload>>(KafkaTopics
                    .OrderStorageRecordsRequests,
                new GetOrderStorageRecordsRequest(Guid.NewGuid(),
                    pvzId,
                    products),
                ct);

        if (!response.IsSuccess)
        {
            throw new BusinessException(response.Errors
                .Select(error => error.Message).ToString());
        }

        return response.Payload!.Items;
    }

    public async Task ReturnProductsToStorage(IEnumerable<ProductQuantity> products, CancellationToken ct)
    {
        await producer.ProduceAsync(
            KafkaTopics.ReturnProductsCommand,
            new ReturnProductsToStorageCommand(products),
            ct);
    }
}