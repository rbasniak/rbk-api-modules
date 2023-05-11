//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace rbkApiModules.Commons.Core.Features.Logging.Diagnostics.Services;

//public abstract class BaseDiagnosticsContext : DbContext
//{
//    public BaseDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
//        : base(options)
//    {
//    }

//    protected BaseDiagnosticsContext(DbContextOptions options)
//    : base(options)
//    {
//    }

//    public DbSet<DiagnosticsEntry> Data { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<DiagnosticsEntry>().Property(x => x.Timestamp).HasConversion(DateTimeWithoutKindConverter.GetConverter());

//        base.OnModelCreating(modelBuilder);
//    }
//}