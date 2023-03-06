using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using rbkApiModules.Commons.Core;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;

namespace rbkApiModules.Identity.Core;

public class CustomTenantListAuthorizeAttribute : RbkAuthorizeAttribute
{
    public static bool AllowAnonymous = false;
    public CustomTenantListAuthorizeAttribute(string claim): base(claim)
    {

    }

    public override void OnAuthorization(AuthorizationFilterContext context)
    {
        if (AllowAnonymous)
        {
            return;
        }

        base.OnAuthorization(context); 
    }
}