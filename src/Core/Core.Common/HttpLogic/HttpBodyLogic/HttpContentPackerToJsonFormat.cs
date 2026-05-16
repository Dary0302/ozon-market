using System.Net.Mime;
using System.Text;
using Core.Common.HttpLogic.HttpBodyLogic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Common.HttpLogic.HttpBodyLogic;

internal class HttpContentPackerToJsonFormat : IHttpContentPacker
{
    public string ContentType => MediaTypeNames.Application.Json;
    
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
    };
    
    public HttpContent PackContent(object body)
    {
        if (body is string stringBody)
        {
            body = JToken.Parse(stringBody);
        }
                
        var serializeBody = JsonConvert.SerializeObject(body, JsonSerializerSettings);
        var content = new StringContent(serializeBody, Encoding.UTF8, ContentType);
        return content;
    }
}