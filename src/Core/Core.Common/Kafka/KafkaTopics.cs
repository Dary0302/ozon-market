namespace Core.Common.Kafka;

public static class KafkaTopics
{
    // OrderService -> StorageService
    public const string CheckStockRequests = "order.check-stock.requests";
    public const string CheckStockResponses = "order.check-stock.responses";

    public const string DeliveryDateRequests = "order.delivery-date.requests";
    public const string DeliveryDateResponses = "order.delivery-date.responses";

    public const string ProductStorageRequests = "order.product-storage.requests";
    public const string ProductStorageResponses = "order.product-storage.responses";
    
    public const string ReduceStockCommand = "order.reduce-stock.command";
    
    public const string ReturnProductsCommand= "order.return-products.command";

    // OrderService -> ProductService
    public const string CalculateAmountRequests = "order.calculate-amount.requests";
    public const string CalculateAmountResponses = "order.calculate-amount.responses";
    
    public const string GetProductsPriceRequests = "order.get-prices.requests";
    public const string GetProductsPriceResponses = "order.get-prices.responses";
}