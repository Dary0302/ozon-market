using Core.Common.HttpLogic.Dto;
using Core.Common.HttpLogic.Services.Interfaces;

namespace Core.Common.HttpLogic.Services;

internal class HttpConnectionService : IHttpConnectionService
{
    private readonly IHttpClientFactory httpClientFactory;
    
    public HttpConnectionService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public HttpClient CreateHttpClient(HttpConnectionData httpConnectionData)
    {
        var httpClient = string.IsNullOrWhiteSpace(httpConnectionData.ClientName)
            ? httpClientFactory.CreateClient()
            : httpClientFactory.CreateClient(httpConnectionData.ClientName);

        if (httpConnectionData.Timeout != null)
        {
            httpClient.Timeout = httpConnectionData.Timeout.Value;
        }
        
        return httpClient;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(
        HttpRequestMessage httpRequestMessage,
        HttpClient httpClient,
        CancellationToken cancellationToken,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead
    )
    {
        var response = await httpClient.SendAsync(httpRequestMessage, completionOption, cancellationToken);
        return response;
    }
}