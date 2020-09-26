using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using rbkApiModules.Diagnostics.Core;
using System;
using System.Linq;
using System.Net;

namespace rbkApiModules.Diagnostics.SqlServer
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiDiagnosticsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerDiagnosticsContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<IDiagnosticsModuleStore, SqlServerDiagnosticsStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiDiagnosticsModule(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(new { Errors = new string[] { error.Error.Message } }))
                                .ConfigureAwait(false);
                    }
                });
            });

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerDiagnosticsContext>())
                {
                    context.Database.EnsureCreated();
                }
            }

            return app;
        }
    }
}
