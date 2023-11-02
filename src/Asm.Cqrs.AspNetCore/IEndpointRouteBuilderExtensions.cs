using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;

namespace Asm.Cqrs.AspNetCore;

/// <summary>
/// Extensions for the <see cref="IEndpointRouteBuilder"/> interface.
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Map a request to a query.
    /// </summary>
    /// <typeparam name="TRequest">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapQuery<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IQuery<TResponse> =>
        endpoints.MapGet(pattern, Handlers.HandleQuery<TRequest, TResponse>);

    /// <summary>
    /// Map a request to a query that returns results in pages..
    /// </summary>
    /// <typeparam name="TRequest">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPagedQuery<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IQuery<PagedResult<TResponse>> =>
        endpoints.MapGet(pattern, Handlers.HandlePagedQuery<TRequest, TResponse>);

    /// <summary>
    /// Map a POST request to a command that creates a resource.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="routeName">The name of the route that can be used to get the newly created resource.</param>
    /// <param name="getRouteParams">A delegate that creates the route parameters.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPostCreate<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string routeName, Func<TResponse, object> getRouteParams) where TRequest : ICommand<TResponse> =>
        endpoints.MapPost(pattern, Handlers.CreateCreateHandler<TRequest, TResponse>(routeName, getRouteParams));

    /// <summary>
    /// Map a PUT request to a command that creates a resource.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="routeName">The name of the route that can be used to get the newly created resource.</param>
    /// <param name="getRouteParams">A delegate that creates the route parameters.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPutCreate<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string routeName, Func<TResponse, object> getRouteParams) where TRequest : ICommand<TResponse> =>
        endpoints.MapPut(pattern, Handlers.CreateCreateHandler<TRequest, TResponse>(routeName, getRouteParams));

    /// <summary>
    /// Maps a request to a command to delete a resource.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapDelete<TRequest>(this IEndpointRouteBuilder endpoints, string pattern) =>
        endpoints.MapDelete(pattern, Handlers.HandleDelete<TRequest>);

    /// <summary>
    /// Maps a request to a command to delete a resource and return a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapDelete<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : ICommand<TResponse> =>
        endpoints.MapDelete(pattern, Handlers.HandleDelete<TRequest, TResponse>);

    /// <summary>
    /// Maps a request to a command to patch a resource.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPatchCommand<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : ICommand<TResponse> =>
        endpoints.MapPatch(pattern, Handlers.HandleCommand<TRequest, TResponse>);

    /// <summary>
    /// Maps a request to a command to put a resource.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPutCommand<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : ICommand<TResponse> =>
        endpoints.MapPut(pattern, Handlers.HandleCommand<TRequest, TResponse>);
}
