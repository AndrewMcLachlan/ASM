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
    /// Gets any additional audiences.
    /// </summary>
    public IEnumerable<string> AdditionalAudiences { get; init; } = [];

    /// <summary>
    /// Gets any additional issuers.
    /// </summary>
    public IEnumerable<string> AdditionalIssuers { get; init; } = [];

    /// <summary>
    /// Gets the client ID.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Gets the authority.
    /// </summary>
    public virtual string Authority { get => Domain; }

    /// <summary>
    /// Gets a value indicating whether to validate the audience. Defaults to <see langword="true"/>;
    /// set to <see langword="false"/> only if you deliberately accept tokens for any audience.
    /// </summary>
    public bool ValidateAudience { get; init; } = true;
}
