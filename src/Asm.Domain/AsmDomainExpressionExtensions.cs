using System.Linq.Expressions;

namespace Asm.Domain;

/// <summary>
/// Extensions for combining boolean predicate expressions so that the result stays translatable by a
/// query provider (e.g. EF Core).
/// </summary>
/// <remarks>
/// Each combinator rebinds the two lambdas onto a single shared parameter, so the combined expression is a
/// well-formed tree rather than an <see cref="Expression.Invoke(Expression, Expression[])"/> that most
/// providers cannot translate.
/// </remarks>
public static class AsmDomainExpressionExtensions
{
    /// <summary>
    /// Combines two predicates with a logical <c>AND</c> (<see cref="Expression.AndAlso(Expression, Expression)"/>).
    /// </summary>
    /// <typeparam name="T">The parameter type of the predicate.</typeparam>
    /// <param name="left">The left predicate.</param>
    /// <param name="right">The right predicate.</param>
    /// <returns>A predicate that is <see langword="true"/> when both operands are.</returns>
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        Combine(left, right, Expression.AndAlso);

    /// <summary>
    /// Combines two predicates with a logical <c>OR</c> (<see cref="Expression.OrElse(Expression, Expression)"/>).
    /// </summary>
    /// <typeparam name="T">The parameter type of the predicate.</typeparam>
    /// <param name="left">The left predicate.</param>
    /// <param name="right">The right predicate.</param>
    /// <returns>A predicate that is <see langword="true"/> when either operand is.</returns>
    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        Combine(left, right, Expression.OrElse);

    /// <summary>
    /// Negates a predicate with a logical <c>NOT</c> (<see cref="Expression.Not(Expression)"/>).
    /// </summary>
    /// <typeparam name="T">The parameter type of the predicate.</typeparam>
    /// <param name="expression">The predicate to negate.</param>
    /// <returns>A predicate that is <see langword="true"/> when the operand is <see langword="false"/>.</returns>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
    }

    private static Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, Func<Expression, Expression, BinaryExpression> merge)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        ParameterExpression parameter = Expression.Parameter(typeof(T), left.Parameters[0].Name);

        Expression leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body);
        Expression rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(merge(leftBody, rightBody), parameter);
    }

    /// <summary>
    /// Rewrites every occurrence of one <see cref="ParameterExpression"/> with a replacement expression so
    /// that two independently-authored lambdas can share a single parameter.
    /// </summary>
    private sealed class ReplaceParameterVisitor(ParameterExpression from, Expression to) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == from ? to : base.VisitParameter(node);
    }
}
