using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext base para a store de bancos relacionais
    /// </summary>
    public abstract class BaseDiagnosticsContext : DbContext
    {
        internal readonly string _connectionString;

        public BaseDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
            : base(options)
        {
        }

        protected BaseDiagnosticsContext(DbContextOptions options)
        : base(options)
        {
        }

        public BaseDiagnosticsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BaseDiagnosticsContext()
        {
        }

        public DbSet<DiagnosticsEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
