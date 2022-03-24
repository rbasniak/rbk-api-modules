using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Relational.Core;

namespace rbkApiModules.Diagnostics.Relational.SQLite
{
    /// <summary>
    /// DBContext para a store de SQLite
    /// </summary>
    public class SQLiteDiagnosticsContext : BaseDiagnosticsContext
    {
        public SQLiteDiagnosticsContext(DbContextOptions<SQLiteDiagnosticsContext> options)
            : base(options)
        {
        }
    }
}
