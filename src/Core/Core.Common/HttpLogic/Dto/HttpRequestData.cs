using System.Net.Mime;

namespace Core.Common.HttpLogic.Dto;

public record HttpRequestData
{
    public required HttpMethod Method { get; init; }
    
    public required string Uri { get; init; }
    
    public object Body { get; init; }

    public string ContentType { get; init; } = MediaTypeNames.Application.Json;
    
    public IDictionary<string, string> HeaderDictionary { get; init; } = new Dictionary<string, string>();
    
    public ICollection<KeyValuePair<string, string>> QueryParameterList { get; init; } = new List<KeyValuePair<string, string>>();
}