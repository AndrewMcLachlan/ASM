namespace Asm.OAuth;

/// <summary>
/// OAuth options.
/// </summary>
public record OAuthOptions
{
    /// <summary>
    /// Gets the domain.
    /// </summary>
    public required string Domain { get; init; }

    /// <summary>
    /// Gets the audience.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Gets the authority.
    /// </summary>
    public virtual string Authority { get => Domain; }

    /// <summary>
    /// Gets a value indicating whether to validate the audience.
    /// </summary>
    public bool ValidateAudience { get; init; }
}
