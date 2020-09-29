using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Demo.Database;
using rbkApiModules.Infrastructure.MediatR;
using Shouldly;
using System;
using Xunit;

namespace rbkApiModules.Demo.Tests
{
    public class CreateUserTests
    {
        protected DbContextOptions<DatabaseContext> SetupInMemoryDatabase(out SqliteConnection connection)
        {
            // In-memory database only exists while the connection is open
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

        [Fact]
        public async void Should_create_new_block()
        {
            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                // Cria o esquema no banco de dados
                SeedInMemoryDatabase(databaseOptions);

                var command = new CreateUser.Command
                {
                    Username = "test",
                    Password = "test"
                };

                CommandResponse response;

                // Comando em teste
                using (var context = new DatabaseContext(databaseOptions))
                {
                    response = await new CreateUser.Handler(context, null).Handle(command, default);
                }

                // Verifica
                response.IsValid.ShouldBeTrue(); 

                using (var context = new DatabaseContext(databaseOptions))
                {
                    var createdEntity = await context.Users
                        .FirstOrDefaultAsync(x => EF.Functions.Like(x.Username, command.Username));

                    createdEntity.ShouldNotBeNull();
                    createdEntity.Username.ShouldBe(command.Username);
                }
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
