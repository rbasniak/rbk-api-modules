using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class StateCacheServiceBuilder<TStatesCache, TEventsCache>
       where TStatesCache : ICacheService
       where TEventsCache : ICacheService
    {
        internal StateCacheServiceBuilder()
        {
        }

        internal async Task Run(HttpContext context, Func<Task> next)
        {
            var statesCache = context.RequestServices.GetService<TStatesCache>();
            var eventsCache = context.RequestServices.GetService<TEventsCache>();

            if (statesCache.IsInitialized && eventsCache.IsInitialized)
            {
                await next.Invoke();
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Content-Type", "application/json");

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new string[] { "Serviço de status não inicializado. Por favor contate o suporte técnico." })).ConfigureAwait(false);
            }
        }
    }

    public static class RequirementCheckMiddlewareExtensions
    {
        public static StateCacheServiceBuilder<TStatesCache, TEventsCache> UseRbkWorkflow<TStatesCache, TEventsCache>(this IApplicationBuilder app)
            where TStatesCache : ICacheService
            where TEventsCache : ICacheService
        {
            var builder = new StateCacheServiceBuilder<TStatesCache, TEventsCache>();

            app.Use(builder.Run);
            return builder;
        }
    }
}
