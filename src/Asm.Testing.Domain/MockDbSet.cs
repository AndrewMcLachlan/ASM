using Asm.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Asm.Testing.Domain;

/// <summary>
/// A mock DbSet for domain entities.
/// </summary>
/// <remarks>
/// <para>
/// This class creates a mocked <see cref="DbSet{TEntity}"/> that supports both synchronous
/// and asynchronous LINQ operations, including projections with <c>.Select()</c> followed
/// by terminal operations like <c>SingleOrDefaultAsync()</c> or <c>ToListAsync()</c>.
/// </para>
/// </remarks>
/// <typeparam name="T">The entity type.</typeparam>
public class MockDbSet<T> : Mock<DbSet<T>> where T : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockDbSet{T}"/> class.
    /// </summary>
    /// <param name="data">The data to return.</param>
    public MockDbSet(IEnumerable<T> data) : this(data.AsQueryable())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockDbSet{T}"/> class.
    /// </summary>
    /// <param name="data">The data to return.</param>
    public MockDbSet(IQueryable<T> data)
    {
        this.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(data.Provider));
        this.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        this.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        this.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
        this.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
    }
}
