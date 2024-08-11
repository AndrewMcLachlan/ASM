namespace Asm.Domain;

/// <summary>
/// A basic entity that can raise domain events.
/// </summary>
public abstract class Entity : IEntity
{
    /// <summary>
    /// Domain events that have been raised by the entity.
    /// </summary>
    public ICollection<IDomainEvent> Events { get; } = [];
}

