namespace Core.Common.HttpLogic.Dto;

public record HttpResponse<TResponse> : BaseHttpResponse
{
    public TResponse? Body { get; set; }
}