using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore;

internal static class Handlers
{
    #region Queries

    internal static async ValueTask<IResult> HandleQuery<TQuery, TResult>([AsParameters] TQuery query, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<TResult> =>
       Results.Ok(await dispatcher.Dispatch(query, cancellationToken));

    internal static async ValueTask<IResult> HandlePagedQuery<TQuery, TResult>([AsParameters] TQuery query, HttpContext http, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<PagedResult<TResult>>
    {
        PagedResult<TResult> result = await dispatcher.Dispatch(query, cancellationToken);

        http.Response.Headers.Append("X-Total-Count", result.Total.ToString());
        return Results.Ok(result.Results);
    }

    #endregion

    #region Command handler factories

    internal static Delegate CreateCreateHandler<TRequest, TResult>(string routeName, Func<TResult, object> getRouteParams, CommandBinding binding) where TRequest : ICommand<TResult> =>
        CreateCreateHandler<TRequest, TResult>(routeName, (_, result) => getRouteParams(result), binding);

    internal static Delegate CreateCreateHandler<TRequest, TResult>(string routeName, Func<TRequest, TResult, object> getRouteParams, CommandBinding binding) where TRequest : ICommand<TResult> =>
        ApplyBinding<TRequest, TResult>(
            async (request, dispatcher, cancellationToken) =>
            {
                var result = await dispatcher.Dispatch(request, cancellationToken);
                return Results.CreatedAtRoute(routeName, getRouteParams(request, result), result);
            },
            binding);

    internal static Delegate CreateCommandHandler<TRequest, TResponse>(int statusCode, CommandBinding binding) where TRequest : ICommand<TResponse> =>
        ApplyBinding<TRequest, TResponse>(
            async (request, dispatcher, cancellationToken) =>
            {
                var result = await dispatcher.Dispatch(request, cancellationToken);
                // 200 uses the idiomatic typed Ok result; other codes (e.g. 202 Accepted with a
                // body) go through Json so the requested status is honoured.
                return statusCode == StatusCodes.Status200OK
                    ? Results.Ok(result)
                    : Results.Json(result, statusCode: statusCode);
            },
            binding);

    /// <summary>
    /// A command that returns no response. The endpoint completes with the framework default
    /// (200 OK, no body) and declares no status code; callers opt into a specific code (202, 204,
    /// …) via <c>.Produces(...)</c> on the returned builder.
    /// </summary>
    internal static Delegate CreateVoidCommandHandler<TRequest>(CommandBinding binding) where TRequest : ICommand =>
        ApplyBinding<TRequest>(
            (request, dispatcher, cancellationToken) => dispatcher.Execute(request, cancellationToken).AsTask(),
            binding);

    internal static Delegate CreateDeleteHandler<TRequest>(CommandBinding binding) where TRequest : ICommand =>
        ApplyBindingResult<TRequest>(
            async (request, dispatcher, cancellationToken) =>
            {
                await dispatcher.Execute(request, cancellationToken);
                return Results.NoContent();
            },
            binding);

    #endregion

    #region Binding

    private static Delegate ApplyBindingResult<TRequest>(Func<TRequest, ICommandDispatcher, CancellationToken, Task<IResult>> func, CommandBinding binding) where TRequest : ICommand =>
        binding switch
        {
            CommandBinding.Body => ([FromBody] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            CommandBinding.Parameters => ([AsParameters] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            _ => func,
        };

    private static Delegate ApplyBinding<TRequest>(Func<TRequest, ICommandDispatcher, CancellationToken, Task> func, CommandBinding binding) where TRequest : ICommand =>
        binding switch
        {
            CommandBinding.Body => ([FromBody] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            CommandBinding.Parameters => ([AsParameters] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            _ => func,
        };

    private static Delegate ApplyBinding<TRequest, TResponse>(Func<TRequest, ICommandDispatcher, CancellationToken, Task<IResult>> func, CommandBinding binding) where TRequest : ICommand<TResponse> =>
        binding switch
        {
            CommandBinding.Body => ([FromBody] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            CommandBinding.Parameters => ([AsParameters] request, dispatcher, cancellationToken) => func(request, dispatcher, cancellationToken),
            _ => func,
        };

    #endregion
}
