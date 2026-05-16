using System.Net.Mime;
using Core.Common.HttpLogic.HttpBodyLogic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Core.Common.HttpLogic.HttpBodyLogic;

internal class JsonUnpackerHttpResponse : IUnpackerHttpResponse
{
    public string ContentType => MediaTypeNames.Application.Json;
    
    private readonly JsonSerializerSettings jsonDeserializationSettings = new()
    {
        ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy() }
    };

    public async Task<TResult?> ExtractContentAsync<TResult>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<TResult>(json, jsonDeserializationSettings);
    }
}