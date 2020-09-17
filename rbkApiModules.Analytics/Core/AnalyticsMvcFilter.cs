using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsMvcFilter : IActionFilter
    {
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
                path = (endpoint as RouteEndpoint).RoutePattern.RawText;
            }
            else
            {
                path = context.HttpContext.Request.Path;
            }

            var action = context.ActionDescriptor as ControllerActionDescriptor;
            if (action != null)
            {
                var attribute = action.MethodInfo.GetCustomAttributes(inherit: true).FirstOrDefault(x => x.GetType() == typeof(ApplicationAreaAttribute));

                if (attribute != null)
                {
                    area = (attribute as ApplicationAreaAttribute).Area;
                }
            }

            context.HttpContext.Items.Add("log-data-area", area);
            context.HttpContext.Items.Add("log-data-path", path);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
 
        }

        
    }
}
