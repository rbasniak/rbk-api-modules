using Microsoft.EntityFrameworkCore;
using rbkApiModules.Analytics.Relational.Core;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Relational.SQLite
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
