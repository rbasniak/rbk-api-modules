using rbkApiModules.Commons.Core.Features.Logging.Diagnostics.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Diagnostics;

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