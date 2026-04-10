// TODO: DONE, REVIEWED

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Commons.Core;

public class DomainOutboxMessagesConfig : IEntityTypeConfiguration<DomainOutboxMessage>
{
    public void Configure(EntityTypeBuilder<DomainOutboxMessage> builder)
    {
        builder.ToTable("DomainOutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.OccurredUtc)
            .IsRequired();

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(50);

        builder.Property(x => x.CausationId)
            .HasMaxLength(50);

        // In production scenario we probably want to use a JSONB column for the payload
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

        // Indices to speed up queries
        builder.HasIndex(x => x.ProcessedUtc);

        builder.HasIndex(x => x.DoNotProcessBeforeUtc);

        // Query-time filters: ProcessedUtc IS NULL, (DoNotProcessBeforeUtc IS NULL OR <= now),
        // (ClaimedUntilUtc IS NULL OR < now), Attempts < @maxAttempts, ORDER BY CreatedUtc LIMIT @batch

        // Primary partial index to drive ORDER BY + LIMIT
        builder.HasIndex(x => x.CreatedUtc)
            .HasFilter($@"""{nameof(DomainOutboxMessage.ProcessedUtc)}"" IS NULL AND ""{nameof(DomainOutboxMessage.IsPoisoned)}"" = FALSE");

        // Help when many messages are delayed via backoff
        builder.HasIndex(x => x.DoNotProcessBeforeUtc);

        // Index for poisoned messages for inspection queries
        builder.HasIndex(x => x.IsPoisoned);
    }
} 