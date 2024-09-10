using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Asm.AspNetCore.Mvc.ActionConstraints;

/// <summary>
/// An action constraint that matches the action name against a regular expression.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RegexActionConstraint : Attribute, IActionConstraint
{
    /// <inheritdoc />
    public int Order => 0;

    /// <summary>
    /// Gets or sets the Regex.
    /// </summary>
    public required string Regex { get; set; }

    /// <inheritdoc />
    public bool Accept(ActionConstraintContext context)
    {
        System.Text.RegularExpressions.Regex regex = new(Regex);

        return regex.IsMatch(context.RouteContext.RouteData.Values["action"] as string ?? String.Empty);
    }
}
