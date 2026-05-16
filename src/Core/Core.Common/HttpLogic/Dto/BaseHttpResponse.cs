using System.Net;
using System.Net.Http.Headers;

namespace Core.Common.HttpLogic.Dto;

public abstract record BaseHttpResponse
{
    public HttpStatusCode StatusCode { get; set; }
    
    public HttpResponseHeaders Headers { get; set; }

    public bool IsSuccessStatusCode
    {
        get
        {
            var statusCode = (int)StatusCode;

            return statusCode >= 200 && statusCode <= 299;
        }
    }
}