namespace Asm.Domain;
public interface IEntity
{
    ICollection<IDomainEvent> Events { get; }
}
