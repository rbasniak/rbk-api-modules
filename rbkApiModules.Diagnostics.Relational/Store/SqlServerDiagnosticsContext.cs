using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    public class SqlServerDiagnosticsContext : BaseDiagnosticsContext
    {
        public SqlServerDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
            : base(options)
        {
        }

        public SqlServerDiagnosticsContext(string connectionString)
            : base(connectionString)
        {
        }

        public SqlServerDiagnosticsContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
