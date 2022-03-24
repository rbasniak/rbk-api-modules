using Microsoft.EntityFrameworkCore;
using rbkApiModules.Logs.Core;
using rbkApiModules.Logs.Relational.Core;

namespace rbkApiModules.Logs.Relational.SqlServer
{
    /// <summary>
    /// DBContext para a store do banco Sql Server
    /// </summary>
    public class SqlServerLogContext : BaseLogContext
    {
        public SqlServerLogContext(DbContextOptions<SqlServerLogContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>().Property(x => x.Message).IsUnicode(false);
            modelBuilder.Entity<LogEntry>().Property(x => x.InputData).IsUnicode(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
