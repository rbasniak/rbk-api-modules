using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using rbkApiModules.Demo.Database;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace rbkApiModules.Demo.Tests
{
    public abstract class BaseDatabaseTestProvider
    {
        protected DbContextOptions<DatabaseContext> SetupInMemoryDatabase(out SqliteConnection connection)
        {
            connection = new SqliteConnection("DataSource=:memory:");

            return new DbContextOptionsBuilder<DatabaseContext>()
                    .UseSqlite(connection)
                    .Options;
        }

        protected void SeedInMemoryDatabase(DbContextOptions<DatabaseContext> databaseOptions)
        {
            using (var context = new DatabaseContext(databaseOptions))
            {
                context.Database.EnsureCreated();
            }
        } 
    }
}
