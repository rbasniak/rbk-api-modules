using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Diagnostics.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace rbkApiModules.Analytics.SqlServer
{
    [ExcludeFromCodeCoverage]
    public static class Builder
    {
        public static void AddSqlServerRbkApiAnalyticsModule(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<ITransactionCounter, TransactionCounter>();

            services.AddTransient<DatabaseAnalyticsInterceptor>();

            services.AddDbContext<SqlServerAnalyticsContext>((scope, options) => options
                .UseSqlServer(connectionString)
                .AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                //.EnableDetailedErrors()
                //.EnableSensitiveDataLogging()
            );

            services.AddTransient<IAnalyticModuleStore, SqlServerAnalyticStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiAnalyticsModule(this IApplicationBuilder app, Action<AnalyticsModuleOptions> configureOptions)
        {
            var options = new AnalyticsModuleOptions();
            configureOptions(options);

            app.UseMiddleware<AnalyticsModuleMiddleware>(options);

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerAnalyticsContext>())
                {
                    context.Database.EnsureCreated();

                    if (context.Data.Count() == 0 && options.SeedSampleDatabase)
                    {
                        Seed.SeedDatabase(context);
                    }
                }
            }

            return app;
        }
    }
}
