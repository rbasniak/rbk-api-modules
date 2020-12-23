using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace rbkApiModules.SharedUI
{
    public static class Builder
    {
        public static void AddRbkSharedUIModule(this IServiceCollection services, Assembly[] blazorRoutingAssemblies, RbkSharedUIModuleOptions options)
        {
            var routeLocator = new BlazorRoutesLocator(blazorRoutingAssemblies);

            options.BaseHref = !String.IsNullOrEmpty(options.BaseHref) ?  "/" + options.BaseHref.Trim('/') : "";

            services.AddSingleton(routeLocator);

            if (options == null)
            {
                options = new RbkSharedUIModuleOptions();
            }

            services.AddSingleton(options);
        }
    }

    public class RbkSharedUIModuleOptions
    {
        public RbkSharedUIModuleOptions()
        {
            CustomRoutes = new List<RouteDefinition>();
        }

        public bool UseDiagnosticsRoutes { get; set; }
        public bool UseAnalyticsRoutes { get; set; }
        public bool UseAuditingRoutes { get; set; }
        public List<RouteDefinition> CustomRoutes { get; set; }
        public string BaseHref { get; set; }

        public string FormatUrl(string url)
        {
            if(string.IsNullOrEmpty(BaseHref))
            {
                return "/" + url.Trim('/');
            }
            else
            {
                return BaseHref + "/" + url.Trim('/');
            }
        }
    }

    public class RouteDefinition
    {
        public RouteDefinition(string url, string name)
        {
            Url = url;
            Name = name;
        }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
