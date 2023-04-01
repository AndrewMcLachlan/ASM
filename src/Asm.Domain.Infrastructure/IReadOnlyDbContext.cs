using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure;

/// <summary>
/// A DB context interface that allows the reading of DbSets, but without save functionality.
/// </summary>
public interface IReadOnlyDbContext
{
    DbSet<T> Set<T>() where T : class;
}
