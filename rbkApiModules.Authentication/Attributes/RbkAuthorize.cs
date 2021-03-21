using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Authentication.AuthenticationGroups
{ 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RbkAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public RbkAuthorizeAttribute()
        {
        }

        public string Claim { get; set; }

        public string Group { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (String.IsNullOrEmpty(Claim) && String.IsNullOrEmpty(Group))
            {
                throw new Exception("Dably setup authorization, you need to specify at least either the needed claim or authroization group");
            }

            var user = context.HttpContext.User;
            var authGroup = context.HttpContext.GetAuthenticationGroup();

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roles = user.Claims.Where(x => x.Type == JwtClaimIdentifiers.Roles).Select(x => x.Value).ToList();

            if ((!String.IsNullOrEmpty(Claim) && !roles.Contains(Claim)) || Group != authGroup)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

}
