namespace Core.Common.Errors;

public enum ErrorStatus
{
    [HttpResponseCode(0)] Unknown,
    [HttpResponseCode(400)] Validation,
    [HttpResponseCode(401)] Unauthorized,
    [HttpResponseCode(403)] Forbidden,
    [HttpResponseCode(404)] NotFound,
    [HttpResponseCode(409)] Conflict,
    [HttpResponseCode(422)] UnprocessableContent,
    [HttpResponseCode(500)] Internal,
    [HttpResponseCode(501)] NotImplemented
}