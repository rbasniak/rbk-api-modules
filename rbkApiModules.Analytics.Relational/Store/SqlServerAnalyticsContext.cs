using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Relational
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SqlServerAnalyticsContext : BaseAnalyticsContext
    {
        public SqlServerAnalyticsContext(DbContextOptions<BaseAnalyticsContext> options)
            : base(options)
        {
        }

        public SqlServerAnalyticsContext(string connectionString)
            : base(connectionString)
        {
        }

        public SqlServerAnalyticsContext()
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
