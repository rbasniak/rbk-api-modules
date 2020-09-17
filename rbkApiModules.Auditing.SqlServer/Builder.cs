using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Auditing.Core;

namespace rbkApiModules.Auditing.SqlServer
{
    public static class Builder
    {
        public static void AddSqlServerRbkApiAuditingModule(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SqlServerAuditingContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<IAuditingModuleStore, SqlServerAuditingStore>();
        }

        public static IApplicationBuilder UseSqlServerRbkApiAuditingModule(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<SqlServerAuditingContext>())
                {
                    context.Database.EnsureCreated();
                }
            }

            return app; 
        }
    }
}
