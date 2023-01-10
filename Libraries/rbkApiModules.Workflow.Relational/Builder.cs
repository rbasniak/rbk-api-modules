using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Workflow.Core;
using System.Net;
using System.Text.Json;

namespace rbkApiModules.Workflow.Relational;

public class StateCacheServiceBuilder<TStatesCache, TEventsCache>
   where TStatesCache : IStatesCacheService
   where TEventsCache : IEventsCacheService
{
    internal StateCacheServiceBuilder()
    {
    }

    internal async Task Run(HttpContext context, Func<Task> next)
    {
        var statesCache = context.RequestServices.GetService<IStatesCacheService>();
        var eventsCache = context.RequestServices.GetService<IEventsCacheService>();

        if (statesCache.IsInitialized && eventsCache.IsInitialized)
        {
            await next.Invoke();
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Content-Type", "application/json");

            await context.Response.WriteAsync(JsonSerializer.Serialize(new string[] { "Serviço de status e/ou eventos não inicializado. Por favor contate o suporte técnico." })).ConfigureAwait(false);
        }
    }
}

public static class RequirementCheckMiddlewareExtensions
{
    public static void AddRbkWorkflow<TStatesCache, TEventsCache>(this IServiceCollection services)
        where TStatesCache : class, IStatesCacheService
        where TEventsCache : class, IEventsCacheService
    {
        services.AddSingleton<TStatesCache>();
        services.AddSingleton<TEventsCache>();
    }

    public static StateCacheServiceBuilder<TStatesCache, TEventsCache> UseRbkWorkflow<TStatesCache, TEventsCache>(this IApplicationBuilder app)
        where TStatesCache : IStatesCacheService
        where TEventsCache : IEventsCacheService
    {
        var builder = new StateCacheServiceBuilder<TStatesCache, TEventsCache>();

        app.Use(builder.Run);
        return builder;
    }
}
