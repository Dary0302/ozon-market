namespace OrderService.Api.Dto;

public class GetAllResponseDto<T>(IEnumerable<T> items, int TotalCount);