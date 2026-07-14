using Asm.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Asm.AspNetCore.Mvc.Tests.ActionConstraints;

[Trait("Category", "Unit")]
public class RegexActionConstraintTests
{
    private static ActionConstraintContext ContextForAction(string action)
    {
        var routeData = new RouteData();
        if (action is not null)
        {
            routeData.Values["action"] = action;
        }

        return new ActionConstraintContext
        {
            RouteContext = new RouteContext(new DefaultHttpContext()) { RouteData = routeData },
        };
    }

    /// <summary>
    /// Given an action name that matches the configured regex
    /// When the constraint is evaluated
    /// Then the action is accepted
    /// </summary>
    [Theory]
    [InlineData("^Index$", "Index")]
    [InlineData("^Post.*", "PostComment")]
    public void ActionMatchingRegexIsAccepted(string regex, string action)
    {
        var constraint = new RegexActionConstraint { Regex = regex };

        Assert.True(constraint.Accept(ContextForAction(action)));
    }

    /// <summary>
    /// Given an action name that does not match the configured regex
    /// When the constraint is evaluated
    /// Then the action is rejected
    /// </summary>
    [Fact]
    public void ActionNotMatchingRegexIsRejected()
    {
        var constraint = new RegexActionConstraint { Regex = "^Index$" };

        Assert.False(constraint.Accept(ContextForAction("Home")));
    }

    /// <summary>
    /// Given route data with no action value
    /// When a non-empty regex is evaluated
    /// Then the missing value is treated as empty and rejected
    /// </summary>
    [Fact]
    public void MissingActionValueIsTreatedAsEmpty()
    {
        var constraint = new RegexActionConstraint { Regex = "^Index$" };

        Assert.False(constraint.Accept(ContextForAction(null)));
    }

    /// <summary>
    /// Given a regex action constraint
    /// When its order is read
    /// Then it is zero
    /// </summary>
    [Fact]
    public void OrderIsZero()
    {
        Assert.Equal(0, new RegexActionConstraint { Regex = ".*" }.Order);
    }
}

[Trait("Category", "Unit")]
public class CatchAllAttributeTests
{
    /// <summary>
    /// Given a catch-all constraint
    /// When any action is evaluated
    /// Then it is always accepted
    /// </summary>
    [Fact]
    public void AcceptAlwaysReturnsTrue()
    {
        Assert.True(new CatchAllAttribute().Accept(new ActionConstraintContext()));
    }

    /// <summary>
    /// Given a catch-all constraint
    /// When its order is read
    /// Then it is zero
    /// </summary>
    [Fact]
    public void OrderIsZero()
    {
        Assert.Equal(0, new CatchAllAttribute().Order);
    }
}
