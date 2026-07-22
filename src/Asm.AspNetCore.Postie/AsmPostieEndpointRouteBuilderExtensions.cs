using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Postie.AspNetCore;

namespace Asm.AspNetCore;

/// <summary>
/// Maps paged CQRS queries to ASP.NET Core minimal API endpoints, dispatching through Postie's
/// mediator-agnostic <see cref="IEndpointDispatcher"/>.
/// </summary>
public static class AsmPostieEndpointRouteBuilderExtensions
{
    private const string QueryHttpMethod = "QUERY";

    /// <summary>
    /// Maps a query whose response is a <see cref="PagedResult{T}"/> to an endpoint that returns
    /// the unwrapped page with the total item count in an <c>X-Total-Count</c> response header.
    /// By default the endpoint is a GET bound from route, query and header values;
    /// <paramref name="method"/> selects POST or the HTTP QUERY method instead, both of which bind
    /// the query from the request body by default.
    /// </summary>
    /// <typeparam name="TRequest">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of each item in the page.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="method">The HTTP method to map. Defaults to <see cref="QueryMethod.Get"/>.</param>
    /// <param name="binding">
    /// How the query is bound. Defaults to the idiomatic binding for <paramref name="method"/>:
    /// <see cref="RequestBinding.Parameters"/> for GET, <see cref="RequestBinding.Body"/> for POST
    /// and QUERY.
    /// </param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="endpoints"/> or <paramref name="pattern"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="method"/> is not a defined <see cref="QueryMethod"/> value, or <paramref name="binding"/> is not a defined <see cref="RequestBinding"/> value.</exception>
    public static RouteHandlerBuilder MapPagedQuery<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, QueryMethod method = QueryMethod.Get, RequestBinding? binding = null) where TRequest : notnull
    {
        ArgumentNullException.ThrowIfNull(endpoints);
        ArgumentNullException.ThrowIfNull(pattern);

        var handler = PagedHandler<TRequest, TResponse>(ResolveBinding(method, binding));

        RouteHandlerBuilder builder = method switch
        {
            QueryMethod.Get => endpoints.MapGet(pattern, handler),
            QueryMethod.Post => endpoints.MapPost(pattern, handler),
            QueryMethod.Query => endpoints.MapMethods(pattern, [QueryHttpMethod], handler),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, $"'{method}' is not a defined {nameof(QueryMethod)} value."),
        };

        return builder.Produces<IEnumerable<TResponse>>();
    }

    private static RequestBinding ResolveBinding(QueryMethod method, RequestBinding? binding)
    {
        if (binding is { } explicitBinding)
        {
            if (explicitBinding is not (RequestBinding.Default or RequestBinding.Body or RequestBinding.Parameters))
            {
                throw new ArgumentOutOfRangeException(nameof(binding), explicitBinding, $"'{explicitBinding}' is not a defined {nameof(RequestBinding)} value.");
            }

            return explicitBinding;
        }

        return method == QueryMethod.Get ? RequestBinding.Parameters : RequestBinding.Body;
    }

    // The binding attribute must sit on the delegate's request parameter for minimal API binding to
    // honour it, so a separate lambda is produced per binding.
    private static Delegate PagedHandler<TRequest, TResponse>(RequestBinding binding) where TRequest : notnull =>
        binding switch
        {
            RequestBinding.Body => async ([FromBody] TRequest request, HttpContext http, IEndpointDispatcher dispatcher, CancellationToken cancellationToken) =>
                await DispatchPaged<TRequest, TResponse>(request, http, dispatcher, cancellationToken),
            RequestBinding.Parameters => async ([AsParameters] TRequest request, HttpContext http, IEndpointDispatcher dispatcher, CancellationToken cancellationToken) =>
                await DispatchPaged<TRequest, TResponse>(request, http, dispatcher, cancellationToken),
            _ => async (TRequest request, HttpContext http, IEndpointDispatcher dispatcher, CancellationToken cancellationToken) =>
                await DispatchPaged<TRequest, TResponse>(request, http, dispatcher, cancellationToken),
        };

    private static async Task<IResult> DispatchPaged<TRequest, TResponse>(TRequest request, HttpContext http, IEndpointDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : notnull
    {
        var result = await dispatcher.DispatchAsync<PagedResult<TResponse>>(request, cancellationToken);

        http.Response.Headers.Append("X-Total-Count", result.Total.ToString());
        return Results.Ok(result.Results);
    }
}
