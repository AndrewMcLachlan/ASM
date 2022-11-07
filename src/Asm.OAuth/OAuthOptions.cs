using System.Diagnostics.CodeAnalysis;

namespace Asm.OAuth;

public class OAuthOptions
{
    [AllowNull]
    public string Domain { get; init; }

    [AllowNull]
    public string Audience { get; init; }

    [AllowNull]
    public string ClientId { get; init; }

    public virtual string Authority { get => Domain; }

    public bool ValidateAudience { get; init; }
}
