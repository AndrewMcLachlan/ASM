namespace Asm.Domain;

/// <summary>
/// An entity that can raise domain events.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Domain events that have been raised by the entity.
    /// </summary>
    ICollection<IDomainEvent> Events { get; }
}
