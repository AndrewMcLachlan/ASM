using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Asm.AspNetCore.Mvc.ActionConstraints;

/// <summary>
/// An action constraint that rewrites the action name.
/// </summary>
/// <param name="actionParameterName">The action parameter name.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RewriteActionAttribute(string actionParameterName) : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <summary>
    /// Gets the action parameter name.
    /// </summary>
    public string ActionParameterName { get; } = actionParameterName;

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        context.RouteContext.RouteData.Values["action"] = context.RouteContext.RouteData.Values[ActionParameterName];
        return true;
    }
}
