using System.Diagnostics.CodeAnalysis;

namespace Asm.OAuth;

public class OAuthOptions
{
    [AllowNull]
    public string Domain { get; set; }

    [AllowNull]
    public string Audience { get; set; }

    [AllowNull]
    public string ClientId { get; set; }

    public virtual string Authority { get => Domain; }
}
