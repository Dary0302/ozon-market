using FluentValidation;
using FluentValidation.AspNetCore;
using  OrderService.Api.Validators;

namespace OrderService.Api.Extensions;

public static class ValidationConfiguration
{
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        return services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<IValidatorMarker>();
    }
}