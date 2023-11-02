using Asm.OAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Asm.AspNetCore.Security;
public class AzureADBearerOptions
{
    public AzureOAuthOptions AzureOAuthOptions { get; set; } = new();

    public JwtBearerEvents Events { get; set; } = new();
}
