using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Infrastructure
{
    public static class HttpContextExtensions
    {
        public static string GetUsername(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.User.Identity.Name.ToLower();
        }
    }
}
