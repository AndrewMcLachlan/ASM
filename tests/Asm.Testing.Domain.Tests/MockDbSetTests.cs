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

    /// <summary>
    /// Given a DbSet created from MockDbSetFactory over three entities
    /// When ToListAsync is awaited twice
    /// Then both calls return all three items and the two results are equal
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactoryToListAsyncTwiceReturnsAllItemsBothTimes()
    {
        DbSet<TestEntity> set = MockDbSetFactory.Create(CreateData()).Object;

        var first = await set.ToListAsync(TestContext.Current.CancellationToken);
        var second = await set.ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, first.Count);
        Assert.Equal(3, second.Count);
        Assert.Equal(first, second);
    }

    /// <summary>
    /// Given a DbSet created directly from a MockDbSet over three entities
    /// When ToListAsync is awaited twice
    /// Then both calls return all three items and the two results are equal
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetToListAsyncTwiceReturnsAllItemsBothTimes()
    {
        DbSet<TestEntity> set = new MockDbSet<TestEntity>(CreateData()).Object;

        var first = await set.ToListAsync(TestContext.Current.CancellationToken);
        var second = await set.ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(3, first.Count);
        Assert.Equal(3, second.Count);
        Assert.Equal(first, second);
    }

    /// <summary>
    /// Given an async-enumerable DbSet created from MockDbSetFactory over three entities
    /// When it is iterated with await foreach twice
    /// Then each iteration enumerates all three items
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactoryAwaitForeachTwiceEnumeratesAllItemsBothTimes()
    {
        IAsyncEnumerable<TestEntity> set = (IAsyncEnumerable<TestEntity>)MockDbSetFactory.Create(CreateData()).Object;

        int firstCount = 0;
        await foreach (var _ in set) firstCount++;

        int secondCount = 0;
        await foreach (var _ in set) secondCount++;

        Assert.Equal(3, firstCount);
        Assert.Equal(3, secondCount);
    }

    /// <summary>
    /// Given a queryable created from MockDbSetFactory over three entities
    /// When Count is evaluated twice
    /// Then both evaluations return three
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void MockDbSetFactoryEnumerateTwiceReturnsAllItemsBothTimes()
    {
        IQueryable<TestEntity> set = MockDbSetFactory.CreateQueryable(CreateData());

        Assert.Equal(3, set.Count());
        Assert.Equal(3, set.Count());
    }

    /// <summary>
    /// Given a queryable created from MockDbSetFactory over three entities
    /// When the entities are projected to their names and awaited with ToListAsync
    /// Then the projected names are returned in order
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public async Task MockDbSetFactoryProjectionWithToListAsyncWorks()
    {
        IQueryable<TestEntity> set = MockDbSetFactory.CreateQueryable(CreateData());

        var names = await set.Select(e => e.Name).ToListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["First", "Second", "Third"], names);
    }
}
