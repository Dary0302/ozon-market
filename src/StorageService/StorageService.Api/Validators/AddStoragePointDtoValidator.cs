using FluentValidation;
using StorageService.Application.Dto;

namespace StorageService.Api.Validators;

public class AddStoragePointDtoValidator : AbstractValidator<AddStoragePointDto>
{
    public AddStoragePointDtoValidator()
    {
        RuleFor(dto => dto.Latitude)
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90)
            .WithMessage("Широта не может быть меньше -90 и больше 90");
        
        RuleFor(dto => dto.Longitude)
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180)
            .WithMessage("Долгота не может быть меньше -180 и больше 190");
    }
}