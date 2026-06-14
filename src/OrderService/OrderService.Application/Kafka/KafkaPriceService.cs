using Core.Common.Kafka;
using Core.Common.Kafka.Contracts;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Contracts.Models;
using Core.Common.Kafka.Contracts.Services;
using Core.Common.Kafka.Interfaces;
using Volo.Abp;

namespace OrderService.Application.Kafka;

public class KafkaProductService(IKafkaRpcClient rpc) : IProductService
{
    public async Task<IEnumerable<ProductPrice>> GetPrices(
        ProductPriceRequest request,
        CancellationToken ct)
    {
        var response =
            await rpc.RequestAsync<GetProductsPriceRequest, KafkaResponse<GetProductsPricePayload>>(KafkaTopics
                    .GetProductsPriceRequests,
                new GetProductsPriceRequest(Guid.NewGuid(),
                    request,
                    ct));

        if (!response.IsSuccess)
            throw new BusinessException(response.Errors
                .Select(error => error.Message).ToString());

        return response
            .Payload!
            .ProductPrices;
    }

    public async Task<decimal> GetAmount(IEnumerable<ProductQuantity> productQuantities, CancellationToken ct)
    {
        var response =
            await rpc.RequestAsync<
                CalculateAmountRequest,
                KafkaResponse<CalculateAmountPayload>>(KafkaTopics
                    .CalculateAmountRequests,
                new CalculateAmountRequest(Guid.NewGuid(),
                    productQuantities,
                    ct));

        if (!response.IsSuccess)
            throw new BusinessException(response.Errors
                .Select(error => error.Message).ToString());

        return response.Payload!.Amount;
    }
}