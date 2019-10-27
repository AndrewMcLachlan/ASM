using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Asm.Security
{
    public interface IPrincipalProvider
    {
        ClaimsPrincipal Principal { get; }
    }
}
