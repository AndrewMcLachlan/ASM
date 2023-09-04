namespace Asm.Domain;
public abstract class Entity : IEntity
{
    public ICollection<IDomainEvent> Events { get; } = new List<IDomainEvent>();
}

