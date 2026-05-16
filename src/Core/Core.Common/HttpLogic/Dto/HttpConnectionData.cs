namespace Core.Common.HttpLogic.Dto;

public record struct HttpConnectionData()
{
    public TimeSpan? Timeout { get; init; } = null;
    
    public CancellationToken CancellationToken { get; init; } = default;
    
    public required string ClientName { get; init; }
}