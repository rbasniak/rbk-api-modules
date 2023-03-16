﻿using Demo2.Domain.Events.Domain;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Relational.EventSourcing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.MyImplementation.Database;

public class EventSourcingContext: DbContext, IEventStoreContext
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
