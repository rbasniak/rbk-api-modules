using Microsoft.EntityFrameworkCore;
using rbkApiModules.Auditing.Core;
using System;

namespace rbkApiModules.Auditing.SqlServer
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    public class SqlServerAuditingContext : DbContext
    {
        private readonly string _connectionString;

        public SqlServerAuditingContext(DbContextOptions<SqlServerAuditingContext> options)
            : base(options)
        {
        }

        public SqlServerAuditingContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlServerAuditingContext()
        {
        }

        public DbSet<StoredEvent> Data { get; set; }

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
