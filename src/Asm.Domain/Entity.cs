using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Asm.Domain
{
    public abstract class Entity<TKey> : IIdentifiable<TKey>
    {
        [Key]
        [NotNull]
        public TKey Id { get; }

        protected Entity(TKey id)
        {
            ArgumentNullException.ThrowIfNull(id);
            Id = id;
        }

        protected Entity()
        {
            Id = default!;
        }
    }
}