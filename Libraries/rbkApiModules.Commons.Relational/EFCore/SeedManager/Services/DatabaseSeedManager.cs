using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Core.Utilities;

namespace rbkApiModules.Commons.Relational;

public class DatabaseSeedManager<T> : IDatabaseSeeder where T : DbContext
{
    private Dictionary<string, SeedInfo<T>> _seed;

    public DatabaseSeedManager()
    {
        _seed = new Dictionary<string, SeedInfo<T>>();
    }

    public Type DbContextType => typeof(T);

    public void AddSeed(string id, SeedInfo<T> data)
    {
        if (_seed == null)
        {
            _seed = new Dictionary<string, SeedInfo<T>>();
        }

        _seed.Add(id, data);
    }

    public void ApplySeed(IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<T>();
            var webHostEnvironment = scope.ServiceProvider.GetService<IWebHostEnvironment>();

            try
            {
                foreach (var kvp in _seed)
                {
                    var useMigrationInProduction = (kvp.Value.EnvironmentUsage & EnvironmentUsage.Production) != 0;
                    var useMigrationInDevelopment = (kvp.Value.EnvironmentUsage & EnvironmentUsage.Development) != 0;
                    var useMigrationInTest = (kvp.Value.EnvironmentUsage & EnvironmentUsage.Testing) != 0;

                    var isProductionEnvironment = webHostEnvironment.IsProduction();
                    var isDevelopmentEnvironment = !webHostEnvironment.IsProduction() && !TestingEnvironmentChecker.IsTestingEnvironment;
                    var isTestingEnvironment = TestingEnvironmentChecker.IsTestingEnvironment;

                    if  (useMigrationInDevelopment && isDevelopmentEnvironment ||
                         useMigrationInProduction && isProductionEnvironment ||
                         useMigrationInTest && isTestingEnvironment)
                    {
                        if (!context.Set<SeedHistory>().Any(x => x.Id == kvp.Key))
                        {
                            try
                            {
                                using (var transaction = context.Database.BeginTransaction())
                                {
                                    kvp.Value.Function(context, scope.ServiceProvider);

                                    context.Add(new SeedHistory(kvp.Key, DateTime.UtcNow));
                                    context.SaveChanges();

                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new DatabaseSeedException(ex, kvp.Key);
                            }
                        }
                    }
                }
            }
            catch (DatabaseSeedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseSeedException(ex, "Preparation");
            }
        }
    }
}