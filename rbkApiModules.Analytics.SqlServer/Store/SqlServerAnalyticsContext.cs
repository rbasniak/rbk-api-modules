using Microsoft.EntityFrameworkCore;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Utilities.EFCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.SqlServer
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SqlServerAnalyticsContext : DbContext
    {
        private readonly string _connectionString;

        public SqlServerAnalyticsContext(DbContextOptions<SqlServerAnalyticsContext> options)
            : base(options)
        {
        }

        public SqlServerAnalyticsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlServerAnalyticsContext()
        {
        }

        public DbSet<AnalyticsEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnalyticsEntry>().Property(x => x.Timestamp).HasConversion(DateTimeWithoutKindConverter.GetConverter());

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
