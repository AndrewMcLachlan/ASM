namespace Asm.Domain;

/// <summary>
/// Marks an entity as being an aggregate root.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AggregateRootAttribute : Attribute
{
}
