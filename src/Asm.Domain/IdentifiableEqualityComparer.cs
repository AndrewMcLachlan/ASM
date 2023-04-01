using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain;

public class IIdentifiableEqualityComparer<TType, TKey> : EqualityComparer<TType> where TType : IIdentifiable<TKey>
{
    public override bool Equals(TType? x, TType? y)
    {
        if (x == null || x.Id == null || y == null || y.Id == null) return false;

        return x.Id.Equals(y.Id);
    }


    public override int GetHashCode([DisallowNull] TType obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        return obj.Id!.GetHashCode();
    }
}