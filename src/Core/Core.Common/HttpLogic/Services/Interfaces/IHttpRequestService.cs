using Core.Common.HttpLogic.Dto;

namespace Core.Common.HttpLogic.Services.Interfaces;

public interface IHttpRequestService
{
    Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(
        HttpRequestData requestData,
        HttpConnectionData connectionData);
}