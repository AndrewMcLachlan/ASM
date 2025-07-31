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
            () => {
                var type = DomainEventHandlerGenericType.MakeGenericType(eventType);
                var method = type.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.Handle))!;
                return (type, method);
            });

        var handlers = serviceProvider.GetServices(handlerType);

        var tasks = handlers.Select(handler =>
            ((ValueTask)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!).AsTask());

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
