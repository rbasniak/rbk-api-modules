using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// DBContext para a store de SqLite
    /// </summary>
    internal class SqLiteContext : DbContext
    {
        private readonly string _connectionString;

        public SqLiteContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<SqliteWebRequest> Requests { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
