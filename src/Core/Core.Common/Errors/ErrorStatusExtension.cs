using System.Reflection;

namespace Core.Common.Errors;

public static class ErrorStatusExtension
{
    public static int GetHttpResponseCode(this ErrorStatus status)
    {
        var memberInfos = status.GetType().GetMember(status.ToString());
        
        if (memberInfos.Length == 0)
            throw new Exception($"Не найден http код для {status}");

        var attributes = memberInfos[0].GetCustomAttributes<HttpResponseCodeAttribute>(false).ToArray();
        
        if (attributes.Length == 0)
            throw new Exception($"Не найден {nameof(HttpResponseCodeAttribute)} для {status}");

        return attributes[0].StatusCode;
    }
}