using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Read;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Relational.CQRS;

namespace Demo1.Database.Read;

public class ReadDatabaseContext : ReadDbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReadDatabaseContext(DbContextOptions<ReadDatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDatabaseContext).Assembly);

        modelBuilder.AddJsonFields();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    } 
}