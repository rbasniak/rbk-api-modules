using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Comments.Relational;
using rbkApiModules.Faqs.Relational;
using rbkApiModules.Notifications.Relational;
using rbkApiModules.Commons.Core.Auditing;
using rbkApiModules.Identity;

namespace Demo1.Database.Domain;

public class DatabaseContext: DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<TraceLog> TraceLog { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeedHistory).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommentConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FaqConfig).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationConfig).Assembly);

        modelBuilder.AddJsonFields();
        modelBuilder.SetupTenants();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    }

    public override int SaveChanges()
    {
        UpdateAuditInfo();

        return base.SaveChanges();
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellation = default)
    {
        UpdateAuditInfo();

        return base.SaveChangesAsync(cancellation);
    }

    private void UpdateAuditInfo()
    {
        ChangeTracker.DetectChanges();

        var addedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
        var updatedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
        var deletedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();

        var username = _httpContextAccessor.HttpContext?.User.Identity.Name;

        foreach (var addedEntity in addedEntries)
        {
            if (addedEntity.Entity is IAuditableEntity entity)
            {
                entity.CreatedAt = DateTime.UtcNow;
                entity.ModifiedAt = DateTime.UtcNow;

                entity.CreatedBy = username;
                entity.ModifiedBy = username;
            }
        }

        foreach (var updatedEntity in updatedEntries)
        {
            if (updatedEntity.Entity is IAuditableEntity entity)
            {
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = username;
            }
        }

        foreach (var deletedEntity in deletedEntries)
        {
            if (deletedEntity.Entity is IAuditableEntity entity)
            {
                // Nothing to do, the entity will be deleted :(
            }
        }
    }
}