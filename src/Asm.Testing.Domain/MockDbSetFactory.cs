using Microsoft.EntityFrameworkCore;
using Moq;

namespace Asm.Testing.Domain;


/// <summary>
/// Helper class for creating mock DbSets and queryables.
/// </summary>
public static class MockDbSetFactory
{
    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> for any class type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <returns>A mock DbSet.</returns>
    public static Mock<DbSet<T>> Create<T>(IEnumerable<T> data) where T : class
    {
        return Create(data.AsQueryable());
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> for any class type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <returns>A mock DbSet.</returns>
    public static Mock<DbSet<T>> Create<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
        mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        return mockSet;
    }

    /// <summary>
    /// Creates an <see cref="IQueryable{T}"/> that supports async operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is useful when you need to inject an <see cref="IQueryable{T}"/> directly
    /// (e.g., for CQRS handlers that receive queryables as dependencies) rather than
    /// mocking a full DbSet.
    /// </para>
    /// <para>
    /// The returned queryable supports all EF Core async operations including
    /// <c>ToListAsync()</c>, <c>SingleOrDefaultAsync()</c>, <c>FirstOrDefaultAsync()</c>, etc.,
    /// as well as projections with <c>.Select()</c>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <returns>An async-capable queryable backed by the mock DbSet.</returns>
    public static IQueryable<T> CreateQueryable<T>(IEnumerable<T> data) where T : class
    {
        return Create(data).Object;
    }

    /// <summary>
    /// Creates an <see cref="IQueryable{T}"/> that supports async operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The data to return.</param>
    /// <returns>An async-capable queryable backed by the mock DbSet.</returns>
    public static IQueryable<T> CreateQueryable<T>(IQueryable<T> data) where T : class
    {
        return Create(data).Object;
    }
}
