using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Comments;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.Utilities.EFCore;

namespace rbkApiModules.Demo.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<BaseUser> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StateGroup> StateGroups { get; set; }
        public DbSet<Transition> Transitions { get; set; }
        public DbSet<StateChangeEvent> StateChangeEvents { get; set; }
        public DbSet<BaseClient> Customers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Client>()
                .Property(x => x.Name)
                .HasColumnName("Name");

            modelBuilder.Entity<Manager>()
                .Property(x => x.Name)
                .HasColumnName("Name");

            modelBuilder.ApplyConfigurations(new [] 
            {
                typeof(DatabaseContext).Assembly,
                typeof(CommentEntity.Command).Assembly,
                typeof(UserLogin.Command).Assembly,
                typeof(CreatePlan.Command).Assembly,
            });
        } 
    }
}
