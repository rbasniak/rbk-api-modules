using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Demo.Database;
using rbkApiModules.Infrastructure.MediatR;
using Shouldly;
using System;
using Xunit;

namespace rbkApiModules.Demo.Tests
{
    public class GetAllUsersTests: BaseDatabaseTestProvider
    { 

        [Fact]
        public async void Should_list_all_users()
        {
            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                // Cria o esquema no banco de dados
                SeedInMemoryDatabase(databaseOptions);




                var moqContext = new Mock<HttpContext>();
                var ihttpContextAccessorMock = new Mock<IHttpContextAccessor>();
                ihttpContextAccessorMock.Setup(x => x.HttpContext).Returns(moqContext.Object);


                var command = new GetAllDemoUsers.Command();

                QueryResponse response;

                // Comando em teste
                using (var context = new DatabaseContext(databaseOptions))
                {
                    response = await new GetAllDemoUsers.Handler(context, ihttpContextAccessorMock.Object).Handle(command, default);
                }

                // Verifica
                response.IsValid.ShouldBeTrue(); 

                //using (var context = new DatabaseContext(databaseOptions))
                //{
                //    var createdEntity = await context.Users
                //        .FirstOrDefaultAsync(x => EF.Functions.Like(x.Username, command.Username));

                //    createdEntity.ShouldNotBeNull();
                //    createdEntity.Username.ShouldBe(command.Username);
                //}
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
