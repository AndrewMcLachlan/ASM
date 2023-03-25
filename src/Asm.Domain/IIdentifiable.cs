using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain
{
    public interface IIdentifiable<T> : IEquatable<IIdentifiable<T>>
    {
        [NotNull]
        T Id { get; }

        bool IEquatable<IIdentifiable<T>>.Equals(IIdentifiable<T>? other)
        {
            if (other == null || Id.Equals(default(T))) return false;

            return other.Id.Equals(Id);
        }
    }
}