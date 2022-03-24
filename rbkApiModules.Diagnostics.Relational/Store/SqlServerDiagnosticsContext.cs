using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SqlServerDiagnosticsContext : BaseDiagnosticsContext
    {
        public SqlServerDiagnosticsContext(DbContextOptions<SqlServerDiagnosticsContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.ExceptionMessage).IsUnicode(false);
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.StackTrace).IsUnicode(false);
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.DatabaseExceptions).IsUnicode(false);
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.InputData).IsUnicode(false);
            modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.ExtraData).IsUnicode(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
