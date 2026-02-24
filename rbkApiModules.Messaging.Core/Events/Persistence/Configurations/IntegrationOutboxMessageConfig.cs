// TODO: DONE, REVIEWED

// TODO: Add conditional indexes

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Commons.Core;

public class IntegrationOutboxMessageConfig : IEntityTypeConfiguration<IntegrationOutboxMessage>
{
    public void Configure(EntityTypeBuilder<IntegrationOutboxMessage> builder)
    {
        builder.ToTable("IntegrationOutboxMessages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OccurredUtc)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(100);

        builder.Property(x => x.CausationId)
            .HasMaxLength(100);

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CreatedUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedUtc);
        
        builder.Property(x => x.Attempts)
            .IsRequired();
        
        builder.Property(x => x.DoNotProcessBeforeUtc);

        builder.Property(x => x.IsPoisoned)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => new { x.TenantId, x.Name, x.Version });
        
        builder.HasIndex(x => x.ProcessedUtc);
        
        builder.HasIndex(x => x.CreatedUtc);
        
        builder.HasIndex(x => x.DoNotProcessBeforeUtc);

        // Index for poisoned messages for inspection queries
        builder.HasIndex(x => x.IsPoisoned);

    }
}
