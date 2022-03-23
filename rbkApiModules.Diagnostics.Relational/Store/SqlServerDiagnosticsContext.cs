using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext para a store de SQL Server
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SqlServerDiagnosticsContext : BaseDiagnosticsContext
    {
        public SqlServerDiagnosticsContext(DbContextOptions<SqlServerDiagnosticsContext> options)
            : base(options)
        {
        }
    }
}
