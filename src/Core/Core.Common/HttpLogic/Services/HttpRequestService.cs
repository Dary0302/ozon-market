using Core.Common.HttpLogic.Dto;
using Core.Common.HttpLogic.HttpBodyLogic.Interfaces;
using Core.Common.HttpLogic.Services.Interfaces;

namespace Core.Common.HttpLogic.Services;

internal class HttpRequestService : IHttpRequestService
{
    private readonly IHttpConnectionService httpConnectionService;
    private readonly IEnumerable<IUnpackerHttpResponse> responseUnpackerList;
    private readonly IEnumerable<IHttpContentPacker> contentPackerList;

    public HttpRequestService(
        IHttpConnectionService httpConnectionService,
        IEnumerable<IUnpackerHttpResponse> responseUnpackerList,
        IEnumerable<IHttpContentPacker> contentPackerList)
    {
        this.httpConnectionService = httpConnectionService;
        this.responseUnpackerList = responseUnpackerList;
        this.contentPackerList = contentPackerList;
    }

    public async Task<HttpResponse<TResponse>> SendRequestAsync<TResponse>(
        HttpRequestData requestData,
        HttpConnectionData connectionData)
    {
        var client = httpConnectionService.CreateHttpClient(connectionData);

        var requestMessage = ConvertToHttpRequestMessage(requestData);
        
        var response = await httpConnectionService.SendRequestAsync(requestMessage, client, connectionData.CancellationToken);
        
        return new HttpResponse<TResponse>
        {
            StatusCode = response.StatusCode,
            Body = await ExtractContentAsync<TResponse>(response),
            Headers = response.Headers,
        };
    }

    private HttpRequestMessage ConvertToHttpRequestMessage(HttpRequestData requestData)
    {
        var uriBuilder = new UriBuilder(requestData.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);

        foreach (var keyValuePair in requestData.QueryParameterList)
            query.Add(keyValuePair.Key, keyValuePair.Value);

        uriBuilder.Query = query.ToString();
        
        var requestMessage = new HttpRequestMessage(requestData.Method, uriBuilder.Uri);
        requestMessage.Content = PackContent(requestData.Body, requestData.ContentType);
        foreach (var keyValuePair in requestData.HeaderDictionary)
            requestMessage.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        return requestMessage;
    }

    private async Task<TResponse?> ExtractContentAsync<TResponse>(HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.ContentType?.MediaType;

        foreach (var responseUnpacker in responseUnpackerList)
            if (responseUnpacker.ContentType == contentType)
                return await responseUnpacker.ExtractContentAsync<TResponse>(response);
        
        throw new InvalidOperationException(
            $"Couldn't find an {nameof(IUnpackerHttpResponse)} for working with ContentType = {contentType}.");
    }

    private HttpContent PackContent(object body, string contentType)
    {
        foreach (var contentPacker in contentPackerList)
            if (contentPacker.ContentType == contentType)
                return contentPacker.PackContent(body);
        
        throw new InvalidOperationException(
            $"Couldn't find an {nameof(IHttpContentPacker)} for working with ContentType = {contentType}.");
    }
}