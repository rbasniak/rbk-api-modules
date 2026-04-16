using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity;
using rbkApiModules.Identity.Core;

namespace Demo2;

public class DatabaseContext : DbContext
{
    private readonly Func<string?> _tenantIdProvider;

    internal DatabaseContext(DbContextOptions<DatabaseContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(tenantProvider);
        _tenantIdProvider = () => tenantProvider.CurrentTenantId;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public DbSet<ApiKey> ApiKeys { get; set; }

    public DbSet<ApiKeyUsageByDay> ApiKeyUsageByDay { get; set; }

    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeedHistory).Assembly);

        modelBuilder.AddJsonFields();
        modelBuilder.SetupTenants();

        modelBuilder.ApplyRbkTenantQueryFilters(_tenantIdProvider, config =>
        {
            config.FilterByTenantOnly<User>();
            config.FilterByTenantOrGlobal<Role>();
            config.FilterByTenantOrGlobal<ApiKey>();
            config.NoFilter<Claim>();
            config.NoFilter<Tenant>();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    }
}
