namespace Asm.Domain;

/// <summary>
/// A domain event handler that runs in the <em>post-save</em> phase: only after the transaction has
/// committed successfully. Use this for external side-effects (sending email, publishing to a message
/// bus, invalidating a cache) that must not happen unless the change is durably persisted.
/// </summary>
/// <typeparam name="TDomainEvent">The event type that this handler handles.</typeparam>
/// <remarks>
/// A post-save handler runs against an already-committed aggregate: there is no rollback if it throws,
/// so post-save handlers must be idempotent and safe to retry. Guaranteed delivery is outbox territory
/// and is out of scope; this interface is a clean foundation for adding an outbox later without changing
/// it. A handler may implement this interface, <see cref="IPreSaveDomainEventHandler{TDomainEvent}"/>, or
/// both; not implementing a phase interface is the opt-out for that phase.
/// </remarks>
public interface IPostSaveDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles the specified domain event after a successful commit.
    /// </summary>
    /// <param name="domainEvent">The domain event to handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
