using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

    public class Class1
    {

    }
}
