using FluentValidation;
using StorageService.Application.Dto;

namespace StorageService.Api.Validators;

public class AddStoredProductDtoValidator : AbstractValidator<AddStoredProductDto>
{
    public AddStoredProductDtoValidator()
    {
        RuleFor(dto => dto.Quantity)
            .GreaterThan(0)
            .WithMessage("Количество не может быть меньше 0");
    }
}