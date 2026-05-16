using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Common.Errors;

public static class ResultExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var error = result.Errors.OfType<AppError>().FirstOrDefault()
                    ?? result.Errors[0] as AppError
                    ?? new AppError(ErrorStatus.Unknown, result.Errors[0].Message);

        return ConvertToActionResult(error);
    }

    private static ObjectResult ConvertToActionResult(AppError error)
    {
        var statusCode = error.Status.GetHttpResponseCode();
        var details = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Status.ToString(),
            Detail = error.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        return new ObjectResult(details) { StatusCode = statusCode };
    }
    
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        var error = result.Errors.OfType<AppError>().FirstOrDefault()
                    ?? result.Errors[0] as AppError
                    ?? new AppError(ErrorStatus.Unknown, result.Errors[0].Message);
        
        return ConvertToActionResult(error);
    }
}