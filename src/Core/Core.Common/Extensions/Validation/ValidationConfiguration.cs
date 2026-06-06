using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Common.Extensions.Validation;

public static class ValidationConfiguration
{
    public static IServiceCollection AddValidation<TMarker>(this IServiceCollection services)
    {
        return services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<TMarker>();
    }
}