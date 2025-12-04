using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core.Helpers;

namespace rbkApiModules.Commons.Relational;

public static class SeedBuilder
{
    public static IApplicationBuilder SetupDatabase<T>(this IApplicationBuilder app, Action<RbkDatabaseSetupOptions> configureOptions)
    where T : DbContext
    {
        var options = new RbkDatabaseSetupOptions();
        configureOptions(options);

        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

        using (var scope = scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<T>();

            if (TestingEnvironmentChecker.IsTestingEnvironment)
            {
                context.Database.EnsureDeleted();

                context.Database.Migrate();
            }

            if (options._migrateOnStartup)
            {
                if (!TestingEnvironmentChecker.IsTestingEnvironment)
                {
#if DEBUG
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("\"__SeedHistory\" already exists"))
                        {
                            context.Database.EnsureDeleted();
                            context.Database.Migrate();
                        }
                        else
                        {
                            throw;
                        }
                    }
#endif

                    context.Database.Migrate();
                }
            }


            context.Dispose();
        }

        return app;
    }

    public static IApplicationBuilder SeedDatabase<T>(this IApplicationBuilder app)
        where T : IDatabaseSeeder
    {
        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

        using (var scope = scopeFactory.CreateScope())
        {
            var seeder = (IDatabaseSeeder)Activator.CreateInstance(typeof(T));

            var context = (DbContext)scope.ServiceProvider.GetService(seeder.DbContextType);

            seeder.ApplySeed(app);

            context.Dispose();
        }

        return app;
    }
}

public class RbkDatabaseSetupOptions
{
    internal bool _migrateOnStartup = true;

    public RbkDatabaseSetupOptions MigrateOnStartup(bool migrateOnStartup = true)
    {
        _migrateOnStartup = migrateOnStartup;
        return this;
    }
}