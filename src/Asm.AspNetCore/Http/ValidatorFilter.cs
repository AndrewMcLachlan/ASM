using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Http;

internal class ValidatorFilter<T>(int parameterIndex) : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? arg = context.GetArgument<T>(parameterIndex);

        IValidator<T>? validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();

        validator.ValidateAndThrow(arg);

        return next(context);
    }
}
