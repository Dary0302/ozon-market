using FluentValidation;
using ProductService.Application.Dto;

namespace ProductService.Api.Validators;

public class AddPhotoRequestValidator : AbstractValidator<AddPhotoDto>
{
    public AddPhotoRequestValidator()
    {
        RuleFor(dto => dto.PhotoData.Length)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Фото должно присутствовать");
    }
}