using System.Linq.Expressions;

namespace Asm.Domain.Tests;

public class AsmDomainExpressionExtensionsTests
{
    /// <summary>
    /// Given two predicate expressions with different parameter names
    /// When they are combined with AndAlso
    /// Then the result shares a single parameter and evaluates as a logical AND
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AndAlsoRebindsParametersAndEvaluates()
    {
        Expression<Func<int, bool>> left = x => x > 2;
        Expression<Func<int, bool>> right = y => y < 5;

        var combined = left.AndAlso(right);
        var predicate = combined.Compile();

        // Both lambdas used different parameter names; the result must share one parameter.
        Assert.Single(combined.Parameters);
        Assert.Equal(System.Linq.Expressions.ExpressionType.AndAlso, combined.Body.NodeType);
        Assert.True(predicate(3));
        Assert.False(predicate(2));
        Assert.False(predicate(5));
    }

    /// <summary>
    /// Given two predicate expressions with different parameter names
    /// When they are combined with OrElse
    /// Then the result evaluates as a logical OR
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void OrElseRebindsParametersAndEvaluates()
    {
        Expression<Func<int, bool>> left = x => x > 4;
        Expression<Func<int, bool>> right = y => y % 2 == 0;

        var predicate = left.OrElse(right).Compile();

        Assert.True(predicate(6));  // even
        Assert.True(predicate(5));  // > 4
        Assert.False(predicate(3)); // neither
    }

    /// <summary>
    /// Given a predicate expression
    /// When Not is applied
    /// Then the compiled predicate returns the negated result
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NotNegatesPredicate()
    {
        Expression<Func<int, bool>> expression = x => x > 2;

        var predicate = expression.Not().Compile();

        Assert.False(predicate(3));
        Assert.True(predicate(1));
    }

    /// <summary>
    /// Given a null expression argument
    /// When AndAlso is called
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void AndAlsoNullArgumentsThrow()
    {
        Expression<Func<int, bool>> expression = x => x > 2;

        Assert.Throws<ArgumentNullException>(() => AsmDomainExpressionExtensions.AndAlso(null!, expression));
        Assert.Throws<ArgumentNullException>(() => expression.AndAlso(null!));
    }

    /// <summary>
    /// Given a null expression argument
    /// When Not is called
    /// Then an ArgumentNullException is thrown
    /// </summary>
    [Fact]
    [Trait("Category", "Unit")]
    public void NotNullArgumentThrows()
    {
        Assert.Throws<ArgumentNullException>(() => AsmDomainExpressionExtensions.Not<int>(null!));
    }
}
