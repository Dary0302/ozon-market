using Core.Common.HttpLogic.HttpBodyLogic;
using Core.Common.HttpLogic.HttpBodyLogic.Interfaces;
using Core.Common.HttpLogic.Services;
using Core.Common.HttpLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Common.HttpLogic;

public static class HttpServiceSetup
{
    public static IServiceCollection AddHttpRequestService(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddHttpClient();
        
        services.TryAddTransient<IHttpRequestService, HttpRequestService>();
        services.AddTransient<IHttpConnectionService, HttpConnectionService>();
        services.AddTransient<IHttpContentPacker, HttpContentPackerToJsonFormat>();
        services.AddTransient<IUnpackerHttpResponse, JsonUnpackerHttpResponse>();
        
        return services;
    }
}