using Asm.Data.Azure;
using Microsoft.Data.SqlClient;

namespace Microsoft.EntityFrameworkCore;

public static class EntityFrameworkCoreAzureDbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAzureAuthenticationProvider(this DbContextOptionsBuilder builder)
    {
        SqlAuthenticationProvider.SetProvider(SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow, new AzureAuthenticationProvider());

        return builder;
    }
}
