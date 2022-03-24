using Microsoft.EntityFrameworkCore;
using rbkApiModules.Logs.Core;
using rbkApiModules.Utilities.EFCore;

namespace rbkApiModules.Logs.Relational.Core
{
    /// <summary>
    /// DBContext base para a store dos bancos de dados relacionais
    /// </summary>
    public abstract class BaseLogContext : DbContext
    {
        public BaseLogContext(DbContextOptions<BaseLogContext> options)
            : base(options)
        {
        }

        protected BaseLogContext(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<LogEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>().Property(x => x.Timestamp).HasConversion(DateTimeWithoutKindConverter.GetConverter());

            base.OnModelCreating(modelBuilder);
        }
    }
}
