using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.EFCore
{
    public static class SeedExtensions
    {
        public class DatabaseSeedCheckMiddleware
        {
            internal DatabaseSeedCheckMiddleware()
            {
            }

            internal async Task Run(HttpContext context, Func<Task> next)
            {
                if (DatabaseSeedManager.SeedSuccess)
                {
                    await next.Invoke();
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Content-Type", "application/json");
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new string[]
                    {
                        "Erro durante o seed do banco de dados. Não foi possível iniciar a aplicação." ,
                        DatabaseSeedManager.SeedException.ToBetterString()
                    })).ConfigureAwait(false);
                }
            }
        }

        public static DatabaseSeedCheckMiddleware UseDatabaseSeedCheck(this IApplicationBuilder app)
        {
            var builder = new DatabaseSeedCheckMiddleware();
            app.Use(builder.Run);
            return builder;
        }
    }
}
