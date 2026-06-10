namespace StorageService.Application.Dto;

public record DecreaseQuantityDto(Guid ProductId, Guid StorageId, int Quantity);