using Demo1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity;

namespace Demo1;

public class DatabaseContext : DbContext
{
    private readonly Func<string?> _tenantIdProvider;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _tenantIdProvider = () => httpContextAccessor.HttpContext?.User.FindFirst("tenant")?.Value?.ToUpperInvariant();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public DbSet<ApiKey> ApiKeys { get; set; }

    public DbSet<ApiKeyUsageByDay> ApiKeyUsageByDay { get; set; }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    public DbSet<Plant> Plants { get; set; }

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
            config.FilterByTenantOnly<Post>();
            config.FilterByTenantOnly<Blog>();
            config.FilterByTenantOrGlobal<ApiKey>();
            config.NoFilter<Claim>();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    }
}