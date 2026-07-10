using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure;

internal class Publisher(IServiceProvider serviceProvider) : IPublisher
{
    private static readonly Type DomainEventHandlerGenericType = typeof(IDomainEventHandler<>);

    // Compiled Handle invokers cached for the process lifetime — avoids per-publish reflection and
    // surfaces a handler's exceptions as their own type rather than a TargetInvocationException.
    private static readonly ConcurrentDictionary<Type, (Type HandlerType, Func<object, object, CancellationToken, ValueTask> Invoker)> Handlers = new();

    public async ValueTask Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var eventType = domainEvent.GetType();

        var (handlerType, invoker) = Handlers.GetOrAdd(eventType, static type =>
        {
            var handlerType = DomainEventHandlerGenericType.MakeGenericType(type);
            var handleMethod = handlerType.GetMethod("Handle")!;

            var handlerParam = Expression.Parameter(typeof(object), "handler");
            var eventParam = Expression.Parameter(typeof(object), "domainEvent");
            var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var call = Expression.Call(
                Expression.Convert(handlerParam, handlerType),
                handleMethod,
                Expression.Convert(eventParam, type),
                cancellationTokenParam);

            var invoker = Expression.Lambda<Func<object, object, CancellationToken, ValueTask>>(call, handlerParam, eventParam, cancellationTokenParam).Compile();
            return (handlerType, invoker);
        });

        var handlers = serviceProvider.GetServices(handlerType);

        // Publish sequentially: handlers commonly share the scoped DomainDbContext that is
        // mid-save, and EF Core forbids concurrent operations on one context instance.
        foreach (var handler in handlers)
        {
            await invoker(handler!, domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
