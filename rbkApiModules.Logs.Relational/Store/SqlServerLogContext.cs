using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Logs.Relational
{
    /// <summary>
    /// DBContext para a store do banco Sql Server
    /// </summary>
    public class SqlServerLogContext : BaseLogContext
    {
        public SqlServerLogContext(DbContextOptions<BaseLogContext> options)
            : base(options)
        {
        }

        public SqlServerLogContext(string connectionString)
            : base(connectionString)
        {
        }

        public SqlServerLogContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }
    }
}
