using System.Reflection;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;

namespace Asm.Domain.Infrastructure;

internal class Publisher(IServiceProvider serviceProvider, IAppCache cache) : IPublisher
{
    private static readonly Type DomainEventHandlerGenericType = typeof(IDomainEventHandler<>);
    private const string HandlerInfoCacheKeyPrefix = "DomainEventHandler_Info_";

    public async ValueTask Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
    {
        var eventType = domainEvent.GetType();

        // Cache both type and method in a single operation
        var (handlerType, handleMethod) = cache.GetOrAdd(
            $"{HandlerInfoCacheKeyPrefix}{eventType.FullName}",
            () =>
            {
                var type = DomainEventHandlerGenericType.MakeGenericType(eventType);
                var method = type.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))!;
                return (type, method);
            });

        var handlers = serviceProvider.GetServices(handlerType);

        // Publish sequentially: handlers commonly share the scoped DomainDbContext that is
        // mid-save, and EF Core forbids concurrent operations on one context instance.
        // DoNotWrapExceptions surfaces a handler's synchronous throw as its own type rather
        // than a TargetInvocationException.
        foreach (var handler in handlers)
        {
            await ((ValueTask)handleMethod.Invoke(handler, BindingFlags.DoNotWrapExceptions, null, [domainEvent, cancellationToken], null)!).ConfigureAwait(false);
        }
    }
}
