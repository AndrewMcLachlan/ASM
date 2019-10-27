using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Asm.Security;
using Microsoft.AspNetCore.Http;

namespace Asm.Web.Mvc.Security
{
    public class HttpContextPrincipalProvider : IPrincipalProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpContextPrincipalProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public ClaimsPrincipal Principal => _contextAccessor.HttpContext.User;
    }
}
