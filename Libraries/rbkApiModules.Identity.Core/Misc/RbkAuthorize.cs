using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RbkAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public RbkAuthorizeAttribute(string claim)
    {
        Claim = claim;
    }

    public string Claim { get; set; }

    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        if (string.IsNullOrEmpty(Claim))
        {
            throw new InvalidOperationException("Badly setup authorization, you need to specify the claim needed to access this resource");
        }

        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var roles = user.Claims.Where(x => x.Type == JwtClaimIdentifiers.Roles).Select(x => x.Value).ToList();

        if (!string.IsNullOrEmpty(Claim) && !roles.Contains(Claim))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}