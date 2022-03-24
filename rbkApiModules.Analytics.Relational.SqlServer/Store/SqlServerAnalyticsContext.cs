using Microsoft.EntityFrameworkCore;
using rbkApiModules.Analytics.Relational.Core;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Relational.SqlServer
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
