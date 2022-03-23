using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// DBContext para a store de SQLite
    /// </summary>
    public class SQLiteDiagnosticsContext : BaseDiagnosticsContext
    {
        public SQLiteDiagnosticsContext(DbContextOptions<BaseDiagnosticsContext> options)
            : base(options)
        {
        }
    }
}
