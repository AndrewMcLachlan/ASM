using System;

namespace Asm.OAuth
{
    public class AzureOAuthOptions : OAuthOptions
    {
        public Guid TenantId { get; set; }

        public override string Authority => $"{Domain}/{TenantId}/v2.0";
    }
}
