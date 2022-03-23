using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Utilities.EFCore;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext base para a store de bancos relacionais
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class BaseDiagnosticsContext : DbContext
    {
        public BaseDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
            : base(options)
        {
        }

        protected BaseDiagnosticsContext(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<DiagnosticsEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.Timestamp).HasConversion(DateTimeWithoutKindConverter.GetConverter());

            base.OnModelCreating(modelBuilder);
        }
    }
}
