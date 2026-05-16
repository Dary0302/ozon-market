namespace Core.Common.Errors;

[AttributeUsage(AttributeTargets.Field)]
public sealed class HttpResponseCodeAttribute(int statusCode) : Attribute
{
    public int StatusCode { get; } = statusCode;
}