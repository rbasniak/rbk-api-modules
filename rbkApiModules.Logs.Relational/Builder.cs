using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Logs.Core;

namespace rbkApiModules.Logs.Relational
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiLogsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerLogContext>((scope, options) => options
                .UseSqlServer(connectionString)
            );

            services.AddTransient<BaseLogContext, SqlServerLogContext>();

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

        public static void AddSQLiteRbkApiLogsModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SQLiteLogContext>((scope, options) => options
                .UseSqlite(connectionString)
            );

            services.AddTransient<BaseLogContext, SQLiteLogContext>();

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
