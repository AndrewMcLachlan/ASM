namespace System.Security.Claims;

/// <summary>
/// Extensions for the <see cref="ClaimsPrincipal"/> class.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the value of a claim from the principal.
    /// </summary>
    /// <typeparam name="T">The type of the claim's value.</typeparam>
    /// <param name="principal">The <see cref="ClaimsPrincipal"/> object that this method extends.</param>
    /// <param name="claimType">The type of claim to return.</param>
    /// <returns>The claim's value or <c>default</c>.</returns>
    public static T? GetClaimValue<T>(this ClaimsPrincipal principal, string claimType)
    {
        var claim = principal.FindFirst(claimType);
        if (claim == null) return default;

        return typeof(T) switch
        {
            var type when type == typeof(Guid) => (T)Convert.ChangeType(Guid.Parse(claim.Value), typeof(T)),
            var type when type == typeof(Guid?) => claim.Value is null ? default : (T)Convert.ChangeType(Guid.Parse(claim.Value), typeof(Guid)),
            var type when type == typeof(int) => (T)Convert.ChangeType(Int32.Parse(claim.Value), typeof(T)),
            _ => (T)Convert.ChangeType(claim.Value, typeof(T)),
        };
    }
}