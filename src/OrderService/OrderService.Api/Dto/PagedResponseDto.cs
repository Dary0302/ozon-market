namespace OrderService.Api.Dto;

public record PagedResponseDto<T>(IEnumerable<T> Items, int TotalCount);