using Asm.AspNetCore;
using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Asm.AspNetCore;
internal static class Handlers
{
    #region Integrated CQRS Handlers
    /* static async ValueTask<IResult> HandleQuery<TQuery, TResult>(IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<TResult>
    {
        var query = Activator.CreateInstance<TQuery>();

        return Results.Ok(await dispatcher.Dispatch(query, cancellationToken));
    }*/

    internal static async ValueTask<IResult> HandleQuery<TQuery, TResult>([AsParameters] TQuery query, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<TResult> =>
       Results.Ok(await dispatcher.Dispatch(query, cancellationToken));

    internal static async ValueTask<IResult> HandlePagedQuery<TQuery, TResult>([AsParameters] TQuery query, HttpContext http, IQueryDispatcher dispatcher, CancellationToken cancellationToken) where TQuery : IQuery<PagedResult<TResult>>
    {
        PagedResult<TResult> result = await dispatcher.Dispatch(query, cancellationToken);

        http.Response.Headers.Append("X-Total-Count", result.Total.ToString());
        return Results.Ok(result.Results);
    }

    internal static async ValueTask<IResult> HandleDelete<TRequest>([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand
    {
        await dispatcher.Dispatch(request!, cancellationToken);

        return Results.NoContent();
    }

    internal static async ValueTask<IResult> HandleDelete<TRequest, TResult>([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand<TResult>
    {
        var result = await dispatcher.Dispatch(request!, cancellationToken);

        return Results.Ok(result);
    }

    internal static ValueTask<TResult> HandleCommand<TRequest, TResult>([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand<TResult> =>
       dispatcher.Dispatch(request!, cancellationToken);

    internal static ValueTask HandleCommand<TRequest>([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) where TRequest : ICommand =>
       dispatcher.Dispatch(request!, cancellationToken);
    #endregion

    #region Advanced CQRS Handlers
    /*internal static Delegate CreateQueryHandler<TRequest, TQuery, TResult>(Func<TRequest, TQuery> func) where TQuery : IQuery<TResult> =>
       async ([AsParameters] TRequest request, IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
       {
           var query = func(request);

           return Results.Ok(await dispatcher.Dispatch(query, cancellationToken));
       };

    internal static Delegate CreateQueryHandler<TRequest>(Func<TRequest, object> createQuery)
    {
        return async ([AsParameters] TRequest request, IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var query = createQuery(request);

            return Results.Ok(await dispatcher.Dispatch(query, cancellationToken));
        };
    }*/

    internal static Delegate CreateCreateHandler<TRequest, TResult>(string routeName, Func<TResult, object> getRouteParams, CommandBinding binding = CommandBinding.None) where TRequest : ICommand<TResult>
    {
        return ParameterBinding<TRequest, TResult>(
            async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
            {
                var result = await dispatcher.Dispatch(request!, cancellationToken);

                return Results.CreatedAtRoute(routeName, getRouteParams(result), result);
            },
            binding
        );
    }

    /*internal static Delegate CreateCreateHandler<TRequest, TCommand, TResult>(Func<TRequest, TCommand> createCommand, string routeName, Func<TResult, object> getRouteParams) where TCommand : ICommand<TResult>
    {
        return async ([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.Dispatch(createCommand(request), cancellationToken);

            return Results.CreatedAtRoute(routeName, getRouteParams(result), result);
        };
    }


    internal static Delegate CreateCreateHandler<TRequest, TCommand, TResult>(Func<TRequest, TCommand> createCommand, Func<TRequest, TResult, Uri> uri) where TCommand : ICommand<TResult>
    {
        return async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.Dispatch(createCommand(request), cancellationToken);

            return Results.Created(uri(request, result), result);
        };
    }

    internal static Delegate CreateCommandHandler<TRequest, TCommand, TResult>(Func<TRequest, TCommand> createCommand) where TCommand : ICommand<TResult>
    {
        return async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            return await dispatcher.Dispatch(createCommand(request), cancellationToken);
        };
    }*/

    internal static Delegate CreateCommandHandler<TRequest>(Func<TRequest, object> createCommand)
    {
        return async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            return await dispatcher.Dispatch(createCommand(request), cancellationToken);
        };
    }

    internal static Delegate CreateCommandHandler<TRequest>(int returnStatusCode, CommandBinding binding = CommandBinding.None) where TRequest : ICommand
    {
        return ParameterBinding(
            async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
            {
                await dispatcher.Dispatch(request, cancellationToken);
                return Results.StatusCode(returnStatusCode);
            },
            binding
        );

    }

    internal static Delegate CreateCommandHandler<TRequest, TResponse>(int returnStatusCode, CommandBinding binding = CommandBinding.None) where TRequest : ICommand<TResponse>
    {
        return ParameterBinding<TRequest, TResponse>(
            async (TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
            {
                await dispatcher.Dispatch(request, cancellationToken);
                return Results.StatusCode(returnStatusCode);
            },
            binding
        );

    }

    internal static Delegate CreateDeleteHandler<TRequest>(Func<TRequest, object> func) =>
       async ([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) =>
       {
           await dispatcher.Dispatch(func(request), cancellationToken);

           return Results.NoContent();
       };
    #endregion


    private static Delegate ParameterBinding<TRequest>(Func<TRequest, ICommandDispatcher, CancellationToken, Task<IResult>> func, CommandBinding binding) where TRequest : ICommand
    {
        return binding switch
        {
            CommandBinding.None => func,
            CommandBinding.Body => ([FromBody] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) => func(request, dispatcher, cancellationToken),
            CommandBinding.Parameters => ([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) => func(request, dispatcher, cancellationToken),
            _ => func
        };
    }

    private static Delegate ParameterBinding<TRequest, TResponse>(Func<TRequest, ICommandDispatcher, CancellationToken, Task<IResult>> func, CommandBinding binding) where TRequest : ICommand<TResponse>
    {
        return binding switch
        {
            CommandBinding.None => func,
            CommandBinding.Body => ([FromBody] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) => func(request, dispatcher, cancellationToken),
            CommandBinding.Parameters => ([AsParameters] TRequest request, ICommandDispatcher dispatcher, CancellationToken cancellationToken) => func(request, dispatcher, cancellationToken),
            _ => func
        };
    }
}
