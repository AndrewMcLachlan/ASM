using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Http;

internal class ValidatorFilter<T>(int parameterIndex) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? arg = context.GetArgument<T>(parameterIndex);

        // A missing/unbindable body is a client error, not a 500 — ValidateAndThrowAsync(null)
        // would otherwise throw ArgumentNullException.
        if (arg is null)
        {
            return Results.Problem(
                title: "A required request body was missing or could not be bound.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        IValidator<T> validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();

        await validator.ValidateAndThrowAsync(arg, context.HttpContext.RequestAborted);

        return await next(context);
    }
}
