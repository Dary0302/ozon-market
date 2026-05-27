namespace OrderService.Domain;

public enum Status
{
    Undefined = 0,
    Created = 1,
    Paid = 2,
    InAssembly = 3,
    TransferredForDelivery = 4,
    Delivered = 5,
    Canceled = 6
}