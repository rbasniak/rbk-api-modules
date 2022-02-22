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
        internal readonly string _connectionString;

        public BaseAnalyticsContext(DbContextOptions<BaseAnalyticsContext> options)
            : base(options)
        {
        }

        public BaseAnalyticsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BaseAnalyticsContext()
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
