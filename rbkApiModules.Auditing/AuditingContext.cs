using Microsoft.EntityFrameworkCore;

namespace AspNetCoreApiTemplate.Auditing
{
    /// <summary>
    /// Classe de contexto do banco de dados de auditoria
    /// </summary>
    public class AuditingContext : DbContext
    {
        public AuditingContext(DbContextOptions<AuditingContext> options)
            : base(options)
        {
        }

        public DbSet<StoredEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
