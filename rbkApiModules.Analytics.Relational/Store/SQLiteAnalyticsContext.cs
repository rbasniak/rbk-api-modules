using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Relational
{
    /// <summary>
    /// DBContext para a store de SQLite
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SQLiteAnalyticsContext : BaseAnalyticsContext
    {
        public SQLiteAnalyticsContext(DbContextOptions<BaseAnalyticsContext> options)
            : base(options)
        {
        }

        public SQLiteAnalyticsContext(string connectionString)
            : base(connectionString)
        {
        }

        public SQLiteAnalyticsContext()
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
