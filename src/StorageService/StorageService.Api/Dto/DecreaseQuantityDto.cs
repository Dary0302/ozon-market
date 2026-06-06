namespace StorageService.Api.Dto;

public record DecreaseQuantityDto(Guid ProductId, Guid StorageId, int Quantity);