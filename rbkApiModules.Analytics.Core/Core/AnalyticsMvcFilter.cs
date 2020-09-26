using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Infrastructure.Api;
using System.Linq;

namespace rbkApiModules.Analytics.Core
{
    // TODO: colocar o filtro nas libs com endpoints
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

            context.HttpContext.Items.Add("log-data-area", area);
            context.HttpContext.Items.Add("log-data-path", path);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
 
        }

        
    }
}
