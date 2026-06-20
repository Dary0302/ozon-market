using FluentValidation;
using ProductService.Application.Dto;
using ProductService.Domain;

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
        RuleFor(dto => dto.Type)
            .IsInEnum()
            .NotEqual(ProductType.Undefined)
            .WithMessage("Тип товара некорректный");
    }
}