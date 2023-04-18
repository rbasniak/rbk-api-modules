using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace rbkApiModules.Commons.Localization;

public class AnalyticsMvcFilter : IActionFilter
{
    public const string LOG_DATA_AREA = "log-data-area";
    public const string LOG_DATA_PATH = "log-data-path";
    public AnalyticsMvcFilter()
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        string path;

        var endpoint = context.HttpContext.Features.Get<IEndpointFeature>().Endpoint;

        if (endpoint is RouteEndpoint)
        {
            path = "/" + (endpoint as RouteEndpoint).RoutePattern.RawText;
        }
        else
        {
            path = "/" + context.HttpContext.Request.Path;
        }

        context.HttpContext.Items.Add(LOG_DATA_PATH, path);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}