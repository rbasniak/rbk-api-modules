using Microsoft.EntityFrameworkCore;
using System;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    public class SqlServerContext : DbContext
    {
        private readonly string _connectionString;

        public SqlServerContext(DbContextOptions<SqlServerContext> options)
            : base(options)
        {
        }

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlServerContext()
        {

        }

        public DbSet<SqlServerWebRequest> Requests { get; set; }

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
