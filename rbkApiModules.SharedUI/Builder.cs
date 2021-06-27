using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.SharedUI
{
    [ExcludeFromCodeCoverage]
    public static class Builder
    {
        //public static void AddSqlServerRbkApiAnalyticsModule(this IServiceCollection services)
        //{

        //}

        public static IApplicationBuilder UseSharedUI(this IApplicationBuilder app, Action<SharedUIModuleOptions> configureOptions)
        {
            var options = new SharedUIModuleOptions();
            configureOptions(options);

            app.UseFileServer(new FileServerOptions
            {
                RequestPath = "/shared-ui",
                FileProvider = new ManifestEmbeddedFileProvider(
                assembly: Assembly.GetAssembly(typeof(Builder)), "UI/dist")
            });

            return app;
        }
    }
}
