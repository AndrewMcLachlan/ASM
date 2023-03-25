using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

public interface IReadOnlyDbContext
{
    DbSet<T> Set<T>() where T : class;
}
