using FluentValidation;
using ProductService.Application.Dto;

namespace ProductService.Api.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductRequestValidator()
    {
        RuleFor(dto => dto.Name.Length)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Имя товара не может быть пустым");
        RuleFor(dto => dto.Description.Length)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Описание товара не может быть пустым");
        RuleFor(dto => (int)dto.Type)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Тип товара некорректный");
        RuleFor(dto => dto.PhotoData.Length)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Фото должно присутствовать");
    }
}