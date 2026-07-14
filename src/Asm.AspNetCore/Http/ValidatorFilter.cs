using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.AspNetCore.Http;

internal class ValidatorFilter<T>(int? parameterIndex = null) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Locate the argument by type when no explicit index is given; fall back to the index for
        // the legacy positional overload.
        T? arg = parameterIndex is int index
            ? context.GetArgument<T>(index)
            : context.Arguments.OfType<T>().FirstOrDefault();

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
