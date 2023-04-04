using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        var path = "unknown";
        var area = "not defined";

        var endpoint = context.HttpContext.Features.Get<IEndpointFeature>().Endpoint;

        if (endpoint is RouteEndpoint)
        {
            path = "/" + (endpoint as RouteEndpoint).RoutePattern.RawText;
        }
        else
        {
            path = "/" + context.HttpContext.Request.Path;
        }

        var action = context.ActionDescriptor as ControllerActionDescriptor;
        if (action != null)
        {
            var controller = action.ControllerTypeInfo;

            var controllerAttribute = action.ControllerTypeInfo.GetCustomAttributes(inherit: true).FirstOrDefault(x => x.GetType() == typeof(ApplicationAreaAttribute));

            var actionAttribute = action.MethodInfo.GetCustomAttributes(inherit: true).FirstOrDefault(x => x.GetType() == typeof(ApplicationAreaAttribute));

            if (actionAttribute != null)
            {
                area = (actionAttribute as ApplicationAreaAttribute).Area;
            }
            else if (controllerAttribute != null)
            {
                area = (controllerAttribute as ApplicationAreaAttribute).Area;
            }
        }

        context.HttpContext.Items.Add(LOG_DATA_AREA, area);
        context.HttpContext.Items.Add(LOG_DATA_PATH, path);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}