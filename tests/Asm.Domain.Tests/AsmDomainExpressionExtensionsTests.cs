using System.Linq.Expressions;

namespace Asm.Domain.Tests;

public class AsmDomainExpressionExtensionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void AndAlso_RebindsParametersAndEvaluates()
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

    [Fact]
    [Trait("Category", "Unit")]
    public void OrElse_RebindsParametersAndEvaluates()
    {
        Expression<Func<int, bool>> left = x => x > 4;
        Expression<Func<int, bool>> right = y => y % 2 == 0;

        var predicate = left.OrElse(right).Compile();

        Assert.True(predicate(6));  // even
        Assert.True(predicate(5));  // > 4
        Assert.False(predicate(3)); // neither
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Not_NegatesPredicate()
    {
        Expression<Func<int, bool>> expression = x => x > 2;

        var predicate = expression.Not().Compile();

        Assert.False(predicate(3));
        Assert.True(predicate(1));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void AndAlso_NullArguments_Throw()
    {
        Expression<Func<int, bool>> expression = x => x > 2;

        Assert.Throws<ArgumentNullException>(() => AsmDomainExpressionExtensions.AndAlso(null!, expression));
        Assert.Throws<ArgumentNullException>(() => expression.AndAlso(null!));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Not_NullArgument_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => AsmDomainExpressionExtensions.Not<int>(null!));
    }
}
