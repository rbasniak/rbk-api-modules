using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication
{
    public static class TokenExtensions
    {
        public static string GetAuthenticationGroup(this HttpContext httpContext)
        {
            if (httpContext == null 
                || httpContext.User == null 
                || httpContext.User.Identity == null)
            {
                throw new Exception("Security breach, non authenticated user");
            }

            var claim = ((System.Security.Claims.ClaimsIdentity)httpContext.User.Identity).Claims
                .FirstOrDefault(c => c.Type.ToLower() == MagicStrings.AUTHENTICATION_GROUP);

            if (claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }

        public static string GetAuthenticationGroup(this IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                throw new Exception("Security breach, non authenticated user");
            }

            return httpContextAccessor.HttpContext.GetAuthenticationGroup();
        }

        //public static string GetDomain(this HttpContext httpContext)
        //{
        //    if (httpContext == null
        //        || httpContext.User == null
        //        || httpContext.User.Identity == null)
        //    {
        //        throw new Exception("Security breach, non authenticated user");
        //    }

        //    var claim = ((System.Security.Claims.ClaimsIdentity)httpContext.User.Identity).Claims
        //        .FirstOrDefault(c => c.Type.ToLower() == MagicStrings.DOMAIN);

        //    if (claim != null)
        //    {
        //        return claim.Value;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
