using FluentValidation;
using OrderService.Api.Dto;
using OrderService.Domain;

namespace OrderService.Api.Validators;

public class StatusDtoValidator : AbstractValidator<StatusDto>
{
    public StatusDtoValidator()
    {
        RuleFor(p => p.Status)
            .Must(status => Enum.IsDefined(typeof(Status), status))
            .WithMessage("Неверный статус заказа");
    }
}