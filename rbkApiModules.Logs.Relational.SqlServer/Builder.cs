using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Logs.Core;
using rbkApiModules.Logs.Relational.Core;

namespace rbkApiModules.Logs.Relational.SqlServer
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiLogsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BaseLogContext, SqlServerLogContext>((scope, options) => options
                .UseSqlServer(connectionString)
            );

            services.AddTransient<ILogsModuleStore, RelationalLogStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiLogsModule(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerLogContext>())
                {
                    context.Database.EnsureCreated();
                }
            }

            return app;
        }
    }
}
