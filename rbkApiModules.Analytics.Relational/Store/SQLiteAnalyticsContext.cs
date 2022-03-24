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
        public SQLiteAnalyticsContext(DbContextOptions<SQLiteAnalyticsContext> options)
            : base(options)
        {
        }
    }
}
