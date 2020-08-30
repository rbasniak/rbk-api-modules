using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Comments;
using rbkApiModules.Infrastructure;
using rbkApiModules.Tester.Models;

namespace rbkApiModules.Tester.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurations(new [] 
            {
                typeof(DatabaseContext).Assembly,
                typeof(CommentEntity.Command).Assembly,
                typeof(UserLogin.Command).Assembly,
            });
        }
    }
}
