using Microsoft.EntityFrameworkCore;
using rbkApiModules.Logs.Core;

namespace rbkApiModules.Logs.Relational
{
    /// <summary>
    /// DBContext base para a store dos bancos de dados relacionais
    /// </summary>
    public abstract class BaseLogContext : DbContext
    {
        internal readonly string _connectionString;

        public BaseLogContext(DbContextOptions<BaseLogContext> options)
            : base(options)
        {
        }

        public BaseLogContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BaseLogContext()
        {
        }

        public DbSet<LogEntry> Data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
