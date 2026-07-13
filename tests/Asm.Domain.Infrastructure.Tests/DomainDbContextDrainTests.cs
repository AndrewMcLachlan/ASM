using System.ComponentModel.DataAnnotations;
using Asm;
using Asm.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Asm.Domain.Infrastructure.Tests;

public class DomainDbContextDrainTests
{
    private class DrainEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public ICollection<IDomainEvent> Events { get; } = [];
    }

    private class DrainEvent : IDomainEvent
    {
    }

    private class DrainDbContext(DbContextOptions options, IPublisher publisher) : DomainDbContext(options, publisher)
    {
        public DbSet<DrainEntity> Entities { get; set; } = null!;
    }

    /// <summary>
    /// Given a handler that re-raises a domain event on every publish
    /// When changes are saved and the event drain runs
    /// Then a BoundExceededException is thrown instead of looping forever
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task RunawayEventCascadeThrowsInsteadOfHanging()
    {
        var entity = new DrainEntity();

        // A pathological handler that re-raises an event on every publish would loop forever;
        // the bounded drain must throw instead.
        var publisher = new Mock<IPublisher>();
        publisher
            .Setup(p => p.PublishPreSave(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .Callback(() => entity.Events.Add(new DrainEvent()))
            .Returns(ValueTask.CompletedTask);

        var options = new DbContextOptionsBuilder<DrainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new DrainDbContext(options, publisher.Object);
        context.Entities.Add(entity);
        entity.Events.Add(new DrainEvent());

        await Assert.ThrowsAsync<BoundExceededException>(
            () => context.SaveChangesAsync(TestContext.Current.CancellationToken));
    }
}
