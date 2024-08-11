using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

/// <summary>
/// A named entity.
/// </summary>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="NamedEntity{TKey}"/> class.
/// </remarks>
/// <param name="id">The ID.</param>
public abstract class NamedEntity<TKey>([DisallowNull] TKey id) : KeyedEntity<TKey>(id), IComparable<NamedEntity<TKey>>
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public required string Name { get; set; }

    /// <inheritdoc/>
    public int CompareTo(NamedEntity<TKey>? other)
    {
        if (other == null) return -1;

        return StringComparer.OrdinalIgnoreCase.Compare(Name, other.Name);
    }
}
