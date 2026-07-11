using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Asm.AspNetCore.Authorisation;

/// <summary>
/// Route parameter authorisation handler.
/// </summary>
/// <param name="httpContextAccessor">An <see cref="IHttpContextAccessor"/> instance.</param>
public abstract class RouteParamAuthorisationHandler<TRequirement>(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<TRequirement> where TRequirement : RouteParamAuthorisationRequirement
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (httpContextAccessor.HttpContext?.Request.RouteValues.TryGetValue(requirement.Name, out var value) == true && value is not null)
        {
            if (await IsAuthorised(value))
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }

    /// <summary>
    /// Performs authorisation based on the route parameter value.
    /// </summary>
    protected abstract ValueTask<bool> IsAuthorised(object value);

}

/// <summary>
/// Route parameter authorisation handler that extracts the route value as a strongly-typed
/// <typeparamref name="TValue"/> before performing authorisation.
/// </summary>
/// <typeparam name="TRequirement">The requirement type.</typeparam>
/// <typeparam name="TValue">The type the route parameter is converted to.</typeparam>
/// <param name="httpContextAccessor">An <see cref="IHttpContextAccessor"/> instance.</param>
public abstract class RouteParamAuthorisationHandler<TRequirement, TValue>(IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<TRequirement> where TRequirement : RouteParamAuthorisationRequirement
{
    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
    {
        if (httpContextAccessor.HttpContext?.Request.RouteValues.TryGetValue(requirement.Name, out var value) == true
            && value is not null
            && TryConvert(value, out var typedValue)
            && await IsAuthorised(typedValue))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }

    /// <summary>
    /// Performs authorisation based on the strongly-typed route parameter value.
    /// </summary>
    /// <param name="value">The converted route parameter value.</param>
    /// <returns><c>true</c> to grant access; otherwise <c>false</c>.</returns>
    protected abstract ValueTask<bool> IsAuthorised(TValue value);

    /// <summary>
    /// Converts the raw route value to <typeparamref name="TValue"/>. Route values are typically
    /// strings; override to supply custom conversion.
    /// </summary>
    /// <param name="value">The raw route value.</param>
    /// <param name="result">The converted value when conversion succeeds.</param>
    /// <returns><c>true</c> if conversion succeeded; otherwise <c>false</c>.</returns>
    protected virtual bool TryConvert(object value, [MaybeNullWhen(false)] out TValue result)
    {
        if (value is TValue typed)
        {
            result = typed;
            return true;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(TValue));
            var stringValue = value as string ?? value.ToString();

            if (stringValue is not null && converter.CanConvertFrom(typeof(string)))
            {
                result = (TValue?)converter.ConvertFromString(null, CultureInfo.InvariantCulture, stringValue);
                return result is not null;
            }
        }
        catch (Exception ex) when (ex is NotSupportedException or FormatException or ArgumentException)
        {
            // Conversion failed; treat as unauthorised.
        }

        result = default;
        return false;
    }
}
