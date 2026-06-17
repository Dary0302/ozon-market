using Core.Common.Kafka;
using Core.Common.Kafka.Contracts.Dto;
using Core.Common.Kafka.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StorageService.Application.Interfaces.Services;
using ProductQuantity = StorageService.Domain.ProductQuantity;

namespace StorageService.Application.Kafka.Consumers;

public class ReturnProductsToStorageCommandConsumer
    : KafkaConsumerService<ReturnProductsToStorageCommand>
{
    private readonly IServiceScopeFactory scopeFactory;
    
    public ReturnProductsToStorageCommandConsumer(
        IOptions<KafkaSettings> settings,
        IServiceScopeFactory scopeFactory)
        : base(settings, KafkaTopics.ReturnProductsCommand, "storage-service")
    {
        this.scopeFactory = scopeFactory;
    }
    
    protected override async Task HandleAsync(ReturnProductsToStorageCommand message, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<IStoredProductService>();
        
        var products =
            message.Products
                .Select(product => new ProductQuantity(product.ProductId, product.Quantity))
                .ToList();

        await service.ReturnProducts(products, token);
    }
}