namespace Asm.Domain;

/// <summary>
/// A domain event handler that runs in the <em>pre-save</em> phase: inside the same transaction as,
/// and immediately before, the write to the database. Use this for in-database reactions that must be
/// persisted atomically with the aggregate that raised the event.
/// </summary>
/// <typeparam name="TDomainEvent">The event type that this handler handles.</typeparam>
/// <remarks>
/// This is the primary domain-event contract. It extends the legacy
/// <see cref="IDomainEventHandler{TDomainEvent}"/> and preserves its behaviour, so existing handlers
/// that implement <see cref="IDomainEventHandler{TDomainEvent}"/> continue to run pre-save unchanged.
/// A handler may implement this interface, <see cref="IPostSaveDomainEventHandler{TDomainEvent}"/>, or
/// both; not implementing a phase interface is the opt-out for that phase.
/// </remarks>
public interface IPreSaveDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
}
