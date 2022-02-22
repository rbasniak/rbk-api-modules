using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext para a store de SQLite
    /// </summary>
    public class SQLiteDiagnosticsContext : BaseDiagnosticsContext
    {
        public SQLiteDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
            : base(options)
        {
        }

        public SQLiteDiagnosticsContext(string connectionString)
            : base(connectionString)
        {
        }

        public SQLiteDiagnosticsContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }
    }
}
