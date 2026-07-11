using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore;

/// <summary>
/// Options controlling the behaviour of the Asm <see cref="ProblemDetailsFactory"/>.
/// </summary>
/// <remarks>
/// Configure via <c>services.Configure&lt;ProblemDetailsFactoryOptions&gt;(...)</c> or the
/// <c>AddProblemDetailsHandler&lt;T&gt;</c> service-collection extension. Because handlers live on an
/// options instance resolved per dependency-injection container, two applications (or two containers)
/// do not share handler state.
/// </remarks>
public class ProblemDetailsFactoryOptions
{
    private readonly Dictionary<Type, Func<HttpContext, IExceptionHandlerFeature, ProblemDetails>> _handlers = [];

    /// <summary>
    /// Gets the registered exception-to-<see cref="ProblemDetails"/> handlers, keyed by exception type.
    /// </summary>
    public IReadOnlyDictionary<Type, Func<HttpContext, IExceptionHandlerFeature, ProblemDetails>> Handlers => _handlers;

    /// <summary>
    /// Adds (or replaces) a custom handler for a specific exception type.
    /// </summary>
    /// <typeparam name="T">The type of exception to handle.</typeparam>
    /// <param name="handler">A delegate that returns a problem details object.</param>
    /// <returns>The same <see cref="ProblemDetailsFactoryOptions"/> instance so that calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="handler"/> is <see langword="null"/>.</exception>
    public ProblemDetailsFactoryOptions AddHandler<T>(Func<HttpContext, IExceptionHandlerFeature, ProblemDetails> handler) where T : Exception
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers[typeof(T)] = handler;
        return this;
    }
}
