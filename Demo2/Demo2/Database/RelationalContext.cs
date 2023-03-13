using Demo2.Database.Config.Relational;
using Demo2.Domain.Events.Domain;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.MyImplementation.Database;

public class RelationalContext: DbContext
{
    public RelationalContext(DbContextOptions<RelationalContext> options) : base(options)
    {
    }

    public DbSet<DomainEventDataObject> Events { get; set; }

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
