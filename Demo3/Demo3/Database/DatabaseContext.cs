using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity;

namespace Demo3;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeedHistory).Assembly);

        modelBuilder.AddJsonFields();
        modelBuilder.SetupTenants();

        // modelBuilder.Entity<User>().Ignore(x => x.Password);
        // modelBuilder.Entity<User>().Ignore(x => x.PasswordRedefineCode);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    } 
}