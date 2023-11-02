namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// TO BE MOVED TO Asm.Web.
/// </summary>
public static class IEndpointConventionBuilderExtensions
{
    public static TBuilder WithNames<TBuilder>(this TBuilder builder, string displayName) where TBuilder : IEndpointConventionBuilder =>
        builder.WithDisplayName(displayName)
               .WithName(displayName.ToMachine());
}
