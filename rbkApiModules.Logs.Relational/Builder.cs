using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Logs.Core;

namespace rbkApiModules.Logs.Relational
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiLogModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerLogContext>((scope, options) => options
                .UseSqlServer(connectionString)
            );

            services.AddTransient<ILogsModuleStore, RelationalLogStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiDiagnosticsModule(this IApplicationBuilder app)
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

        public static void AddSQLiteRbkApiLogModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SQLiteLogContext>((scope, options) => options
                .UseSqlite(connectionString)
            );

            services.AddTransient<ILogsModuleStore, RelationalLogStore>();
        }

        public static IApplicationBuilder UseSQLiteRbkApiDiagnosticsModule(this IApplicationBuilder app)
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
