using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

public class KeyedEntityEqualityComparer<TType, TKey> : EqualityComparer<TType> where TType : KeyedEntity<TKey>
{
    public override bool Equals(TType? x, TType? y) =>
        x == y;

    public override int GetHashCode([DisallowNull] TType obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return obj.Id!.GetHashCode();
    }
}
