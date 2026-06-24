using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Implementations;
using Core.Common.Kafka.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using StorageService.Domain;

namespace StorageService.Application.Kafka.Consumers;

public class ReduceCountOfProductsCommandConsumer
    : KafkaConsumerService<ReduceCountOfProductsCommand>
{
    private readonly IServiceScopeFactory scopeFactory;
    
    public ReduceCountOfProductsCommandConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory)
        : base(settings, KafkaTopics.ReduceStockCommand, "storage-service")
    {
        this.scopeFactory = scopeFactory;
    }
    
    
    protected override async Task HandleAsync(ReduceCountOfProductsCommand message, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<IStoredProductService>();
        
        var products =
            message.Items
                .Select(product => new DecreaseQuantity(product.ProductId, product.StorageId, product.Quantity))
                .ToList();

        await service.DecreaseStoredProductQuantity(products, token);
    }
}