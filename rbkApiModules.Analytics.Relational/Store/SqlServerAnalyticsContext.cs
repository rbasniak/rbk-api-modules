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
        public SqlServerAnalyticsContext(DbContextOptions<SqlServerAnalyticsContext> options)
            : base(options)
        {
        }
    }
}
