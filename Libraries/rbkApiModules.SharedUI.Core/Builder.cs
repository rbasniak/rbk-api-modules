using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace rbkApiModules.SharedUI.Core;

public static class Builder
{
    public static void AddRbkSharedUIModule(this IServiceCollection services, IConfiguration Configuration)
    {
        services.Configure<RbkSharedUIAuthentication>(Configuration.GetSection(nameof(RbkSharedUIAuthentication)));
    }

    public static IApplicationBuilder UseRbkSharedUIModule(this IApplicationBuilder app, Action<SharedUIModuleOptions> configureOptions)
    {
        var options = new SharedUIModuleOptions();
        configureOptions(options);

        app.UseFileServer(new FileServerOptions
        {
            RequestPath = "/shared-ui",
            FileProvider = new ManifestEmbeddedFileProvider(assembly: Assembly.GetAssembly(typeof(Builder)), "UI/dist"),
        });

        app.MapWhen((context) => context.Request.Path.StartsWithSegments("/shared-ui"), (appBuilder) =>
        {
            app.UseStaticFiles();
            appBuilder.UseRouting();
            appBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapFallbackToFile("/shared-ui/index.html");
            });
        });

        return app;
    }
}
