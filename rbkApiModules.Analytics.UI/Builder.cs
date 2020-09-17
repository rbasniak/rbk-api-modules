using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Analytics.UI
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiAnalyticsUIModule(this IServiceCollection services)
        {
            
        }

        public static IApplicationBuilder UseRbkApiAnalyticsUIModule(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
