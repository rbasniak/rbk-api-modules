using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Logs.Core;
using rbkApiModules.Logs.Relational.Core;

namespace rbkApiModules.Logs.Relational.SQLite
{
    public static class Builder
    {
        public static void AddSQLiteRbkApiLogsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BaseLogContext, SQLiteLogContext>((scope, options) => options
                .UseSqlite(connectionString)
            );

            services.AddTransient<ILogsModuleStore, RelationalLogStore>();
        }

        public static IApplicationBuilder UseSQLiteRbkApiLogsModule(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SQLiteLogContext>())
                {
                    context.Database.EnsureCreated();
                }
            }

            return app;
        }
    }
}
