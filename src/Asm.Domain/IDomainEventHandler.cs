namespace Asm.Domain;

/// <summary>
/// A domain event handler.
/// </summary>
/// <typeparam name="TDomainEvent">The event type that this handler handles.</typeparam>
public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
