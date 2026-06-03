using OrderService.Domain;
using FluentValidation;
using OrderService.Api.Dto;

namespace OrderService.Api.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(dto => dto.PvzId)
            .NotEmpty()
            .WithMessage("Id пункта выдачи заказов не может быть пустым");

        RuleFor(dto => dto.ClientAmount)
            .GreaterThan(0)
            .WithMessage("Сумма заказа должна быть больше нуля");

        RuleFor(dto => dto.Products)
            .NotEmpty()
            .WithMessage("Список позиций не может быть пустым");

        RuleForEach(dto => dto.Products)
            .ChildRules(item =>
            {
                item.RuleFor(product => product.ProductId)
                    .NotEmpty()
                    .WithMessage("Id товара не может быть пустым");

                item.RuleFor(product => product.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Количество товаров должно быть больше 0");
            });
    }
}