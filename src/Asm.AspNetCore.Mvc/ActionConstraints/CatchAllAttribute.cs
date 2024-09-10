using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Asm.AspNetCore.Mvc.ActionConstraints;

/// <summary>
/// An action constraint that matches any action.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CatchAllAttribute : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        return true;
        //context.RouteContext.RouteData.Values["action"];
    }
}
