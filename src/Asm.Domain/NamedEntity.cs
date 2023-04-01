using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

public abstract class NamedEntity<TKey> : KeyedEntity<TKey>, IComparable<NamedEntity<TKey>>
{
    protected NamedEntity([DisallowNull] TKey id) : base(id)
    {
    }

    public required string Name { get; set; }

    public int CompareTo(NamedEntity<TKey>? other)
    {
        if (other == null) return -1;

        return StringComparer.OrdinalIgnoreCase.Compare(Name, other.Name);
    }
}
