using Microsoft.EntityFrameworkCore;
using rbkApiModules.Logs.Relational.Core;

namespace rbkApiModules.Logs.Relational.SQLite
{
    /// <summary>
    /// DBContext para a store do banco SQLite
    /// </summary>
    public class SQLiteLogContext : BaseLogContext
    {
        public SQLiteLogContext(DbContextOptions<SQLiteLogContext> options)
            : base(options)
        {
        }
    }
}
