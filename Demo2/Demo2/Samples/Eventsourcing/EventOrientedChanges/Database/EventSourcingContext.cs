using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Config;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Relational.EventSourcing;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Database;

public class EventSourcingContext : DbContext, IEventStoreContext
{
    public EventSourcingContext(DbContextOptions<EventSourcingContext> options) : base(options)
    {
    }

    public DbSet<DomainEventDataObject> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        //optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
        //{
        //    builder.AddFilter(_ => false);
        //}));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DomainEventDataObjectConfig());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeedHistory).Assembly);

        modelBuilder.AddJsonFields();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    }
}
