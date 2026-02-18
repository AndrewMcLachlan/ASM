using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure.Tests;

internal class TestDbContext(DbContextOptions options, IPublisher publisher) : DomainDbContext(options, publisher)
{
    public DbSet<TestEntity> TestEntities { get; set; }
}
