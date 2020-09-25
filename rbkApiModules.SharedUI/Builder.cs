using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace rbkApiModules.SharedUI
{
    public static class Builder
    {
        public static void AddRbkSharedUIModule(this IServiceCollection services, Assembly[] blazorRoutingAssemblies)
        {
            var routeLocator = new BlazorRoutesLocator(blazorRoutingAssemblies);
            services.AddSingleton(routeLocator);
        }
    }
}
