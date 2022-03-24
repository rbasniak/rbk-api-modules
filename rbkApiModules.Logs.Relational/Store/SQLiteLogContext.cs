using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Logs.Relational
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
