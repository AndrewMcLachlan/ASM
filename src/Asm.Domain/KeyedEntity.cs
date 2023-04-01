using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

namespace Asm.Domain;

/// <summary>
/// Base class for a keyed entity.
/// </summary>
/// <typeparam name="TKey">The type of the entity key.</typeparam>
public abstract class KeyedEntity<TKey> : IIdentifiable<TKey>
{
    [Key]
    [DisallowNull]
    public TKey Id { get; }

    protected KeyedEntity([DisallowNull]TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
    }

    public override bool Equals(object? obj) => ((IEquatable<IIdentifiable<TKey>>)this).Equals(obj as IIdentifiable<TKey>);

    public static bool operator ==(KeyedEntity<TKey>? a, KeyedEntity<TKey>? b) => (a is null && b is null) || (a?.Equals(b) ?? false);

    public static bool operator !=(KeyedEntity<TKey>? a, KeyedEntity<TKey>? b) => !(a == b);

    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }
}