using Asm.AspNetCore.Http;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="RouteHandlerBuilder"/>.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a validation filter to the route handler.
    /// </summary>
    /// <typeparam name="T">The type of parameter to be validated.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder" /> object that this method extends.</param>
    /// <param name="parameterIndex">The index of the parameter to be validated.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> instance so that calls can be chained.</returns>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder, int parameterIndex = 0)
    {
        return builder.AddEndpointFilter(new ValidatorFilter<T>(parameterIndex));
    }
}
