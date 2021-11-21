using System.ComponentModel.DataAnnotations;

namespace Asm.Domain
{
    public abstract class Entity<TKey> : IIdentifiable<TKey>
    {
        [Key]
        public TKey Id { get; }

        protected Entity(TKey id)
        {
            Id = id;
        }

        protected Entity()
        {
            Id = default!;
        }
    }
}