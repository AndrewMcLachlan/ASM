using Asm.AspNetCore.Http;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="RouteHandlerBuilder"/>.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    /// Adds a validation filter to the route handler that locates the parameter to validate by its type.
    /// </summary>
    /// <typeparam name="T">The type of parameter to be validated.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder" /> object that this method extends.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> instance so that calls can be chained.</returns>
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter(new ValidatorFilter<T>());
    }

    /// <summary>
    /// Adds a validation filter to the route handler for the parameter at the given index.
    /// </summary>
    /// <typeparam name="T">The type of parameter to be validated.</typeparam>
    /// <param name="builder">The <see cref="RouteHandlerBuilder" /> object that this method extends.</param>
    /// <param name="parameterIndex">The index of the parameter to be validated.</param>
    /// <returns>The <see cref="RouteHandlerBuilder"/> instance so that calls can be chained.</returns>
    [Obsolete("Locate the parameter by type using WithValidation<T>() instead. This overload will be removed in a future version.")]
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder, int parameterIndex)
    {
        return builder.AddEndpointFilter(new ValidatorFilter<T>(parameterIndex));
    }
}
