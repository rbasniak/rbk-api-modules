using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using rbkApiModules.Commons.Core;
using Microsoft.AspNetCore.Mvc.Core;
using Microsoft.AspNetCore.Mvc;

namespace rbkApiModules.Identity.Core;

public class WindowsAuthenticationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext filterContext)
    {

    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var actionAllowAnonymous = filterContext.ActionDescriptor.EndpointMetadata
            .Any(x => x.GetType() == typeof(AllowAnonymousAttribute));

        var hasBearerToken = filterContext.HttpContext.Request.Headers.Authorization.ToString().ToLower().StartsWith("bearer ");

        if (filterContext.HttpContext.Request.Path != "/api/authentication/login" && !actionAllowAnonymous && !hasBearerToken)
        {
            filterContext.Result = new UnauthorizedResult();
        }
    }
}