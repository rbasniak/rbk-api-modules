using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Comments;
using rbkApiModules.Infrastructure;

namespace rbkApiModules.Tester.Database
{
    public class DatabaseContext : DbContext
    {
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
                typeof(CreateUser.Command).Assembly,
            });
        }
    }
}
