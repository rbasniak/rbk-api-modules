using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Analytics.Core;

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

        public static AnalyticsModuleBuilder UseSqlServerRbkApiAnalyticsModule(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerAnalyticsContext>())
                {
                    context.Database.EnsureCreated();
                }
            }

            var builder = new AnalyticsModuleBuilder();
            app.Use(builder.Run);
            return builder; 
        }
    }
}
