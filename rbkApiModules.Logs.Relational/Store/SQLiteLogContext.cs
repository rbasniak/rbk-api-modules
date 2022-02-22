using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Logs.Relational
{
    /// <summary>
    /// DBContext para a store do banco SQLite
    /// </summary>
    public class SQLiteLogContext : BaseLogContext
    {
        public SQLiteLogContext(DbContextOptions<BaseLogContext> options)
            : base(options)
        {
        }

        public SQLiteLogContext(string connectionString)
            : base(connectionString)
        {
        }

        public SQLiteLogContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(base._connectionString))
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }
    }
}
