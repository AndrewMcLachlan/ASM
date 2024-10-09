using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Http;

internal class ValidatorFilter<T>(int parameterIndex) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? arg = context.GetArgument<T>(parameterIndex);

        IValidator<T>? validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();

        await validator.ValidateAndThrowAsync(arg);

        return await next(context);
    }
}
