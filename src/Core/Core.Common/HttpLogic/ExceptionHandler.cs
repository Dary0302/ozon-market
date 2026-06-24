using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace Core.Common.HttpLogic;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred");

        (httpContext.Response.StatusCode, var message) = exception switch
        {
            BusinessException ex => (StatusCodes.Status422UnprocessableEntity, ex.Message),
            TimeoutException ex => (StatusCodes.Status504GatewayTimeout, ex.Message),
            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "Запрос был отменён"),
            _ => (StatusCodes.Status500InternalServerError, "Упс! Что-то пошло не так.")
        };

        await httpContext.Response.WriteAsJsonAsync(new { error = message }, cancellationToken: cancellationToken);
        return true;
    }
}