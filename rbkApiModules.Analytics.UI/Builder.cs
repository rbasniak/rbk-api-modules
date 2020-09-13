using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Analytics
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiAnalyticsModule(this IServiceCollection services)
        {
            
        }

        public static IApplicationBuilder UseRbkApiAnalyticsModule(this IApplicationBuilder app)
        {
            

            return app;
        }
    }
}
