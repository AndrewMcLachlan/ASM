using MediatR;

namespace Asm.Domain;

/// <summary>
/// A domain event handler.
/// </summary>
/// <typeparam name="TDomainEvent">The event type that this handler handles.</typeparam>
public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
}
