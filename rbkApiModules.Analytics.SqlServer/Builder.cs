﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Analytics.Core;
using System;
using System.Linq;

namespace rbkApiModules.Analytics.SqlServer
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiAnalyticsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerAnalyticsContext>(options =>
                options.UseSqlServer(connectionString));

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

                    if (context.Data.Count() == 0)
                    {
                        Seed.SeedDatabase(context);
                    }
                }
            }

            return app;
        }
    }
}
