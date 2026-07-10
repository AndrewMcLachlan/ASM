namespace Asm.Domain;

/// <summary>
/// A basic entity that can raise domain events.
/// </summary>
public abstract class Entity : IEntity
{
    private List<IDomainEvent>? _events;

    /// <summary>
    /// Domain events that have been raised by the entity.
    /// </summary>
    /// <remarks>
    /// The backing list is allocated lazily on first access, so entities materialised from queries
    /// (which never raise events) don't pay for it.
    /// </remarks>
    public ICollection<IDomainEvent> Events => _events ??= [];
}

