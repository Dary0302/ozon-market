using Microsoft.AspNetCore.Diagnostics;

namespace ProductService.Api;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new { error = "Упс! Что-то пошло не так." }, cancellationToken: cancellationToken);
        return true;
    }
}