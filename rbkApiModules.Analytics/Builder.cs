using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
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
            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true; // optional
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
        }

        public static IApplicationBuilder UseRbkApiAnalyticsModule(this IApplicationBuilder app)
        {
            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            return app;
        }
    }
}
