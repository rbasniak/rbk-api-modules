﻿using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Models;
using Demo2.Samples.Relational.Database.Config.Relational;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;

namespace Demo2.Samples.Relational.Database;

public class RelationalContext : DbContext
{
    public RelationalContext(DbContextOptions<RelationalContext> options) : base(options)
    {
    }

    public DbSet<DomainEventDataObject> Events { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
        {
            builder.AddFilter(_ => false);
        }));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AttachmentConfig());
        modelBuilder.ApplyConfiguration(new AttachmentTypeConfig());
        modelBuilder.ApplyConfiguration(new ChangeRequestConfig());
        modelBuilder.ApplyConfiguration(new ChangeRequestPriorityConfig());
        modelBuilder.ApplyConfiguration(new ChangeRequestSourceConfig());
        modelBuilder.ApplyConfiguration(new ChangeRequestToDisciplineConfig());
        modelBuilder.ApplyConfiguration(new ChangeRequestTypeConfig());
        modelBuilder.ApplyConfiguration(new DisciplineConfig());
        modelBuilder.ApplyConfiguration(new DocumentCategoryConfig());
        modelBuilder.ApplyConfiguration(new DocumentConfig());
        modelBuilder.ApplyConfiguration(new EvidenceAttachmentConfig());
        modelBuilder.ApplyConfiguration(new FicCategoryConfig());
        modelBuilder.ApplyConfiguration(new FicCategoryConfig());
        modelBuilder.ApplyConfiguration(new PlatformConfig());
        modelBuilder.ApplyConfiguration(new StateConfig());
        modelBuilder.ApplyConfiguration(new UnConfig());

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeedHistory).Assembly);

        modelBuilder.AddJsonFields();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeWithoutKindConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableDateTimeWithoutKindConverter>();
    }
}