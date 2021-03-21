using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Comments;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Utilities.EFCore;
using rbkApiModules.Utilities.Extensions;
using rbkApiModules.Workflow; 

namespace rbkApiModules.Demo.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ClientUser> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StateGroup> StateGroups { get; set; }
        public DbSet<Transition> Transitions { get; set; }
        public DbSet<StateChangeEvent> StateChangeEvents { get; set; }

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
