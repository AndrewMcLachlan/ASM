using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Asm.Domain.Infrastructure.Tests;
internal class TestDbContext(DbContextOptions options, IMediator mediator) : DomainDbContext(options, mediator)
{
    public DbSet<TestEntity> TestEntities { get; set; }
}
