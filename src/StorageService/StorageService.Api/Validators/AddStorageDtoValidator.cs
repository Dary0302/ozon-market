using FluentValidation;
using StorageService.Application.Dto;

namespace StorageService.Api.Validators;

public class AddStorageDtoValidator : AbstractValidator<AddStorageDto>
{
    public AddStorageDtoValidator()
    {
        RuleFor(dto => dto.Address.Length)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Адрес не может быть пустым");
    }
}