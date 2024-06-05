using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
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

        var authorizationAttribute = filterContext.ActionDescriptor.EndpointMetadata
            .FirstOrDefault(x => x.GetType() == typeof(AuthorizeAttribute));

        var requiresApiKey = false;
        if (authorizationAttribute != null)
        {
            requiresApiKey = ((AuthorizeAttribute)authorizationAttribute).AuthenticationSchemes == RbkAuthenticationSchemes.API_KEY;
        }

        var hasBearerToken = filterContext.HttpContext.Request.Headers.Authorization.ToString().ToLower().StartsWith("bearer ");

        if (filterContext.HttpContext.Request.Path != "/api/authentication/login" && !actionAllowAnonymous && !hasBearerToken && !requiresApiKey)
        {
            filterContext.Result = new UnauthorizedResult();
        }
    }
}