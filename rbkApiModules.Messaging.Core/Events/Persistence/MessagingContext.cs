// TODO: DONE, REVIEWED

using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Core;

public class MessagingDbContext : DbContext
{
    public const string DefaultSchema = "messaging";

    public MessagingDbContext(DbContextOptions<MessagingDbContext> options) : base(options) 
    { 
    }

    public DbSet<DomainOutboxMessage> DomainOutboxMessage => Set<DomainOutboxMessage>();
    public DbSet<IntegrationOutboxMessage> IntegrationOutboxMessage => Set<IntegrationOutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DomainOutboxMessagesConfig());
        modelBuilder.ApplyConfiguration(new IntegrationOutboxMessageConfig());
        modelBuilder.ApplyConfiguration(new InboxMessageConfig());
    }
}