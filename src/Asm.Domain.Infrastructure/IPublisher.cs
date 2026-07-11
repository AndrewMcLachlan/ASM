namespace Asm.Domain.Infrastructure;

/// <summary>
/// A publisher for domain events.
/// </summary>
/// <remarks>
/// Each domain event is dispatched in two phases: <see cref="PublishPreSave{TDomainEvent}"/> runs the
/// pre-save handlers (<see cref="IPreSaveDomainEventHandler{TDomainEvent}"/>, and the legacy
/// <see cref="IDomainEventHandler{TDomainEvent}"/>) inside the transaction before the write, and
/// <see cref="PublishPostSave{TDomainEvent}"/> runs the post-save handlers
/// (<see cref="IPostSaveDomainEventHandler{TDomainEvent}"/>) after a successful commit.
/// </remarks>
public interface IPublisher
{
    /// <summary>
    /// Publishes a domain event to its pre-save handlers, which run inside the transaction before the
    /// write to the database.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of event to publish.</typeparam>
    /// <param name="domainEvent">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask PublishPreSave<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Publishes a domain event to its post-save handlers, which run only after the transaction has
    /// committed successfully.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of event to publish.</typeparam>
    /// <param name="domainEvent">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask PublishPostSave<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;

    /// <summary>
    /// Publishes a domain event asynchronously.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of event to publish.</typeparam>
    /// <param name="domainEvent">The event to publish.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Obsolete("Use " + nameof(PublishPreSave) + ". Publish dispatches to pre-save handlers only, preserving pre-v4 behaviour.")]
    ValueTask Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent
        => PublishPreSave(domainEvent, cancellationToken);
}
