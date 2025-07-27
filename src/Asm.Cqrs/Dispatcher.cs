using Asm.Cqrs.Commands;
using Asm.Cqrs.Queries;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Cqrs;

internal class Dispatcher(IServiceProvider serviceProvider, IAppCache cache) : IQueryDispatcher, ICommandDispatcher
{
    private static readonly Type QueryHandlerGenericType = typeof(IQueryHandler<,>);
    private static readonly Type CommandHandlerGenericType = typeof(ICommandHandler<,>);
    private static readonly Type CommandHandlerVoidGenericType = typeof(ICommandHandler<>);

    // Cache key prefix for combined caching
    private const string HandlerInfoCacheKeyPrefix = "HandlerInfo_";

    public ValueTask<TResponse> Dispatch<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var responseType = typeof(TResponse);

        var (handlerType, handleMethod) = cache.GetOrAdd(
            $"{HandlerInfoCacheKeyPrefix}Query_{queryType.FullName}_{responseType.FullName}",
            () => {
                var type = QueryHandlerGenericType.MakeGenericType(queryType, responseType);
                var method = type.GetMethod(nameof(IQueryHandler<IQuery<TResponse>, TResponse>.Handle))!;
                return (type, method);
            });

        var handler = serviceProvider.GetRequiredService(handlerType);
        var result = handleMethod.Invoke(handler, [query, cancellationToken]);

        return (ValueTask<TResponse>)result!;
    }

    public ValueTask<TResponse> Dispatch<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var responseType = typeof(TResponse);

        // Cache both type and method in a single operation
        var (handlerType, handleMethod) = cache.GetOrAdd(
            $"{HandlerInfoCacheKeyPrefix}Command_{commandType.FullName}_{responseType.FullName}",
            () => {
                var type = CommandHandlerGenericType.MakeGenericType(commandType, responseType);
                var method = type.GetMethod(nameof(ICommandHandler<ICommand<TResponse>, TResponse>.Handle))!;
                return (type, method);
            });

        var handler = serviceProvider.GetRequiredService(handlerType);
        var result = handleMethod.Invoke(handler, [command, cancellationToken]);

        return (ValueTask<TResponse>)result!;
    }

    public ValueTask Dispatch(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        // Cache both type and method in a single operation
        var (handlerType, handleMethod) = cache.GetOrAdd(
            $"{HandlerInfoCacheKeyPrefix}CommandVoid_{commandType.FullName}",
            () => {
                var type = CommandHandlerVoidGenericType.MakeGenericType(commandType);
                var method = type.GetMethod(nameof(ICommandHandler<ICommand>.Handle))!;
                return (type, method);
            });

        var handler = serviceProvider.GetRequiredService(handlerType);
        var result = handleMethod.Invoke(handler, [command, cancellationToken]);

        return (ValueTask)result!;
    }
}
