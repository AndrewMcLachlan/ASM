namespace Asm.Domain;

/// <summary>
/// A domain event handler.
/// </summary>
/// <typeparam name="TDomainEvent">The event type that this handler handles.</typeparam>
/// <remarks>
/// This is the base domain-event contract, retained for backwards compatibility. Handlers that
/// implement it run in the pre-save phase (inside the transaction, before the write). New handlers
/// should implement <see cref="IPreSaveDomainEventHandler{TDomainEvent}"/> (which extends this
/// interface) to opt into the pre-save phase, and/or <see cref="IPostSaveDomainEventHandler{TDomainEvent}"/>
/// to run after a successful commit.
/// </remarks>
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
