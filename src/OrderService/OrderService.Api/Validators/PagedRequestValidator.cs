using FluentValidation;
using OrderService.Api.Dto;

namespace OrderService.Api.Validators;

public class PagedRequestValidator : AbstractValidator<PagedRequestDto>
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Номер страницы должен быть не меньше 1");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Размер страницы должен быть не меньше 1");
    }
}