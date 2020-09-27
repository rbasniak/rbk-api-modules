using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Diagnostics.Core;
using System;

namespace rbkApiModules.Diagnostics.SqlServer
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    public class SqlServerDiagnosticsContext : DbContext
    {
        private readonly string _connectionString;

        public SqlServerDiagnosticsContext(DbContextOptions<SqlServerDiagnosticsContext> options)
            : base(options)
        {
        }

        public SqlServerDiagnosticsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlServerDiagnosticsContext()
        {
        }

        public DbSet<DiagnosticsEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!String.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
