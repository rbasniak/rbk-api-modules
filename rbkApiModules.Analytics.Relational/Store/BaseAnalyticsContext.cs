using Microsoft.EntityFrameworkCore;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Utilities.EFCore;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Relational
{
    /// <summary>
    /// DBContext base para a store de bancos relacionais
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class BaseAnalyticsContext : DbContext
    {
        public BaseAnalyticsContext(DbContextOptions<BaseAnalyticsContext> options)
            : base(options)
        {
        }

        protected BaseAnalyticsContext(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<AnalyticsEntry> Data { get; set; }
        public DbSet<SessionEntry> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalyticsEntry>().Property(x => x.Timestamp).HasConversion(DateTimeWithoutKindConverter.GetConverter());

            base.OnModelCreating(modelBuilder);
        }
    }
}
