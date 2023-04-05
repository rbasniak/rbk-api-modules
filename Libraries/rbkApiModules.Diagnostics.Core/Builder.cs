//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using rbkApiModules.Commons.Core.Features.Logging.Diagnostics.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace rbkApiModules.Commons.Diagnostics;

//public static class Builder
//{
//    public static void AddSqlServerRbkApiDiagnosticsModule(this IServiceCollection services, string connectionString)
//    {
//        services.AddTransient<DatabaseDiagnosticsInterceptor>();

//        services.AddDbContext<BaseDiagnosticsContext, SqlServerDiagnosticsContext>((scope, options) => options
//            .UseSqlServer(connectionString)
//            .AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
//        //.EnableDetailedErrors()
//        //.EnableSensitiveDataLogging()
//        );

//        services.AddTransient<IDiagnosticsModuleStore, RelationalDiagnosticsStore>();
//    }

//    public static IApplicationBuilder UseSqlServerRbkApiDiagnosticsModule(this IApplicationBuilder app)
//    {
//        app.UseExceptionHandler(builder =>
//        {
//            builder.Run(async context =>
//            {
//                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

//                var errorHandler = context.Features.Get<IExceptionHandlerFeature>();
//                if (errorHandler != null)
//                {
//                    var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

//                    // Must create a new scope because if we have any errors while saving the diagnostics data, the
//                    // invalid data will be kept in the context and EF will tries to save it again
//                    using (var scope = scopeFactory.CreateScope())
//                    {
//                        using (var database = scope.ServiceProvider.GetService<SqlServerDiagnosticsContext>())
//                        {
//                            var data = new DiagnosticsEntry(context, "GlobalExceptionHandler", errorHandler.Error, null);

//                            await database.AddAsync(data);
//                            await database.SaveChangesAsync();
//                        }
//                    }

//                    await context.Response.WriteAsync(
//                        JsonConvert.SerializeObject(new { Errors = new string[] { "Erro interno no servidor." } }))
//                            .ConfigureAwait(false);
//                }
//            });
//        });

//        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

//        using (var scope = scopeFactory.CreateScope())
//        {
//            using (var context = scope.ServiceProvider.GetService<SqlServerDiagnosticsContext>())
//            {
//                context.Database.EnsureCreated();
//            }
//        }

//        return app;
//    }
//}