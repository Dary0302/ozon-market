using Core.Common.HttpLogic.Dto;

namespace Core.Common.HttpLogic.Services.Interfaces;

internal interface IHttpConnectionService
{
    HttpClient CreateHttpClient(HttpConnectionData httpConnectionData);
    
    Task<HttpResponseMessage> SendRequestAsync(
        HttpRequestMessage httpRequestMessage,
        HttpClient httpClient,
        CancellationToken cancellationToken,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead
    );
}