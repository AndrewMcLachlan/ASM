using Asm.Domain;
using Asm.Testing.Domain;
using Microsoft.EntityFrameworkCore;

namespace Asm.Testing.Domain.Tests;

public sealed class TestEntity : Entity
{
    public required string Name { get; init; }
}

public class MockDbSetTests
{
    private static List<TestEntity> CreateData() =>
    [
        new() { Name = "First" },
        new() { Name = "Second" },
        new() { Name = "Third" },
    ];

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactory_ToListAsyncTwice_ReturnsAllItemsBothTimes()
    {
        DbSet<TestEntity> set = MockDbSetFactory.Create(CreateData()).Object;

        var first = await set.ToListAsync(TestContext.Current.CancellationToken);
        var second = await set.ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, first.Count);
        Assert.Equal(3, second.Count);
        Assert.Equal(first, second);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSet_ToListAsyncTwice_ReturnsAllItemsBothTimes()
    {
        DbSet<TestEntity> set = new MockDbSet<TestEntity>(CreateData()).Object;

        var first = await set.ToListAsync(TestContext.Current.CancellationToken);
        var second = await set.ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, first.Count);
        Assert.Equal(3, second.Count);
        Assert.Equal(first, second);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactory_AwaitForeachTwice_EnumeratesAllItemsBothTimes()
    {
        IAsyncEnumerable<TestEntity> set = (IAsyncEnumerable<TestEntity>)MockDbSetFactory.Create(CreateData()).Object;

        int firstCount = 0;
        await foreach (var _ in set) firstCount++;

        int secondCount = 0;
        await foreach (var _ in set) secondCount++;

        Assert.Equal(3, firstCount);
        Assert.Equal(3, secondCount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void MockDbSetFactory_EnumerateTwice_ReturnsAllItemsBothTimes()
    {
        IQueryable<TestEntity> set = MockDbSetFactory.CreateQueryable(CreateData());

        Assert.Equal(3, set.Count());
        Assert.Equal(3, set.Count());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactory_ProjectionWithToListAsync_Works()
    {
        IQueryable<TestEntity> set = MockDbSetFactory.CreateQueryable(CreateData());

        var names = await set.Select(e => e.Name).ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["First", "Second", "Third"], names);
    }
}
