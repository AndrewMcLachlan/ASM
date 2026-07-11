using Asm.AspNetCore;
using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;

namespace Asm.AspNetCore;

/// <summary>
/// Extensions for the <see cref="IEndpointRouteBuilder"/> interface.
/// </summary>
/// <remarks>
/// Every command-mapping method takes a <see cref="CommandBinding"/> that defaults to
/// <see cref="CommandBinding.Parameters"/> (<c>[AsParameters]</c>). Pass
/// <see cref="CommandBinding.Body"/> to bind the whole request from the JSON body, or
/// <see cref="CommandBinding.None"/> to let the framework decide.
/// </remarks>
public static class AsmCqrsAspNetCoreEndpointRouteBuilderExtensions
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
        endpoints.MapGet(pattern, Handlers.HandleQuery<TRequest, TResponse>)
                 .Produces<TResponse>();

    /// <summary>
    /// Map a request to a query that returns results in pages.
    /// </summary>
    /// <typeparam name="TRequest">The type of the query.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPagedQuery<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern) where TRequest : IQuery<PagedResult<TResponse>> =>
        endpoints.MapGet(pattern, Handlers.HandlePagedQuery<TRequest, TResponse>)
                 .Produces<IEnumerable<TResponse>>();

    /// <summary>
    /// Map a POST request to a command that creates a resource, returning 201 Created.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="routeName">The name of the route that can be used to get the newly created resource.</param>
    /// <param name="getRouteParams">A delegate that creates the route parameters from the response.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPostCreate<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string routeName, Func<TResponse, object> getRouteParams, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPost(pattern, Handlers.CreateCreateHandler<TRequest, TResponse>(routeName, getRouteParams, binding))
                 .Produces<TResponse>(StatusCodes.Status201Created);

    /// <summary>
    /// Map a POST request to a command that creates a resource, returning 201 Created.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="routeName">The name of the route that can be used to get the newly created resource.</param>
    /// <param name="getRouteParams">A delegate that creates the route parameters from the command and the response.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPostCreate<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string routeName, Func<TRequest, TResponse, object> getRouteParams, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPost(pattern, Handlers.CreateCreateHandler<TRequest, TResponse>(routeName, getRouteParams, binding))
                 .Produces<TResponse>(StatusCodes.Status201Created);

    /// <summary>
    /// Map a PUT request to a command that creates a resource, returning 201 Created.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="routeName">The name of the route that can be used to get the newly created resource.</param>
    /// <param name="getRouteParams">A delegate that creates the route parameters from the response.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPutCreate<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, string routeName, Func<TResponse, object> getRouteParams, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPut(pattern, Handlers.CreateCreateHandler<TRequest, TResponse>(routeName, getRouteParams, binding))
                 .Produces<TResponse>(StatusCodes.Status201Created);

    /// <summary>
    /// Maps a request to a command to delete a resource and returns an empty 204 No Content response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapDelete<TRequest>(this IEndpointRouteBuilder endpoints, string pattern, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand =>
        endpoints.MapDelete(pattern, Handlers.CreateDeleteHandler<TRequest>(binding))
                 .Produces(StatusCodes.Status204NoContent);

    /// <summary>
    /// Maps a request to a command to delete a resource and return a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="statusCode">The success status code for the response body. Defaults to 200 OK.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapDelete<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, int statusCode = StatusCodes.Status200OK, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapDelete(pattern, Handlers.CreateCommandHandler<TRequest, TResponse>(statusCode, binding))
                 .Produces<TResponse>(statusCode);

    /// <summary>
    /// Maps a POST request to a command and returns a response with 200 OK.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="statusCode">The success status code for the response body. Defaults to 200 OK.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapCommand<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, int statusCode = StatusCodes.Status200OK, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPost(pattern, Handlers.CreateCommandHandler<TRequest, TResponse>(statusCode, binding))
                 .Produces<TResponse>(statusCode);

    /// <summary>
    /// Maps a POST request to a command that returns no response.
    /// </summary>
    /// <remarks>
    /// No response status is declared: the appropriate code depends on what the endpoint means
    /// (e.g. 200, 202 or 204). The endpoint completes as 200 OK with no body; declare a different
    /// code on the returned <see cref="RouteHandlerBuilder"/> with <c>.Produces(...)</c>.
    /// </remarks>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapCommand<TRequest>(this IEndpointRouteBuilder endpoints, string pattern, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand =>
        endpoints.MapPost(pattern, Handlers.CreateVoidCommandHandler<TRequest>(binding));

    /// <summary>
    /// Maps a PATCH request to a command and returns a response with 200 OK.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="statusCode">The success status code for the response body. Defaults to 200 OK.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPatchCommand<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, int statusCode = StatusCodes.Status200OK, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPatch(pattern, Handlers.CreateCommandHandler<TRequest, TResponse>(statusCode, binding))
                 .Produces<TResponse>(statusCode);

    /// <summary>
    /// Maps a PUT request to a command and returns a response with 200 OK.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="statusCode">The success status code for the response body. Defaults to 200 OK.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPutCommand<TRequest, TResponse>(this IEndpointRouteBuilder endpoints, string pattern, int statusCode = StatusCodes.Status200OK, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand<TResponse> =>
        endpoints.MapPut(pattern, Handlers.CreateCommandHandler<TRequest, TResponse>(statusCode, binding))
                 .Produces<TResponse>(statusCode);

    /// <summary>
    /// Maps a PUT request to a command that returns no response.
    /// </summary>
    /// <remarks>
    /// No response status is declared: the appropriate code depends on what the endpoint means
    /// (e.g. 200, 201 or 204). The endpoint completes as 200 OK with no body; declare a different
    /// code on the returned <see cref="RouteHandlerBuilder"/> with <c>.Produces(...)</c>.
    /// </remarks>
    /// <typeparam name="TRequest">The type of the command.</typeparam>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="binding">How the handler should bind the request. Defaults to <see cref="CommandBinding.Parameters"/>.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customise the endpoint.</returns>
    public static RouteHandlerBuilder MapPutCommand<TRequest>(this IEndpointRouteBuilder endpoints, string pattern, CommandBinding binding = CommandBinding.Parameters) where TRequest : ICommand =>
        endpoints.MapPut(pattern, Handlers.CreateVoidCommandHandler<TRequest>(binding));
}
