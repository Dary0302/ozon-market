using FluentResults;

namespace Core.Common.Errors;

public class AppError : Error
{
    public ErrorStatus Status { get; }

    public AppError(ErrorStatus status, string message) : base(message)
    {
        Status = status;
        Metadata["Error"] = this;
    }

    public static AppError Forbidden(string message = "Доступ запрещен") => new(ErrorStatus.Forbidden, message);
    public static AppError Unauthorized(string message = "Аутентификация не удалась") => new(ErrorStatus.Unauthorized, message);
    public static AppError Validation(string message = "Ошибка валидации данных") => new(ErrorStatus.Validation, message);
    public static AppError Conflict(string message = "Объект уже существует") => new(ErrorStatus.Conflict, message);
    public static AppError NotFound(string message = "Объект не найден") => new(ErrorStatus.NotFound, message);
    public static AppError UnprocessableContent(string massage = "Некорректные данные") => new(ErrorStatus.UnprocessableContent, massage);
    public static AppError NotImplemented(string message = "Не реализовано") => new(ErrorStatus.NotImplemented, message);
}