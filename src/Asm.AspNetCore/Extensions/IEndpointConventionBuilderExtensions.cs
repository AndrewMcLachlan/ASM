namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for the <see cref="IEndpointConventionBuilder"/> interface.
/// </summary>
public static class IEndpointConventionBuilderExtensions
{
    /// <summary>
    /// Adds a name and display name to the endpoint.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <param name="builder">The <see cref="IEndpointConventionBuilder"/> that this method extends.</param>
    /// <param name="displayName">The display name.</param>
    /// <returns>The <see cref="IEndpointConventionBuilder"/> so that calls can be chained.</returns>
    public static TBuilder WithNames<TBuilder>(this TBuilder builder, string displayName) where TBuilder : IEndpointConventionBuilder =>
        builder.WithDisplayName(displayName)
               .WithName(displayName.ToMachine());
}
