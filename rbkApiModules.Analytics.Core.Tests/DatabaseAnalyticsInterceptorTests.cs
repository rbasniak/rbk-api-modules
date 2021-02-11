using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Moq.AutoMock;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using rbkApiModules.Utilities.Testing;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class DatabaseAnalyticsInterceptorTests
    {
        [AutoNamedFact]
        [Trait(TraitTokens.DOMAIN, nameof(DatabaseAnalyticsInterceptor))]
        public async void Should_Record_DB_Statistics_Under_Normal_Conditions()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            // Just for coverage, to test situations in which HttpContext is null
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((DefaultHttpContext)null);

            var databaseOptions = SetupInMemoryDatabase(httpContextAccessorMock.Object, out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                // Reset the HttpContext to simulate a new request
                httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

                using (var context = new TestDbContext(databaseOptions))
                {
                    connection.Open();
                    
                    // Add several items to add up the transactions
                    context.Users.Add(new User { Id = 1 });
                    context.SaveChanges();

                    context.Users.Add(new User { Id = 2 });
                    context.Users.Add(new User { Id = 3 });
                    context.Users.Add(new User { Id = 4 });
                    context.SaveChanges();

                    // Add an async reader
                    await context.Users.FirstAsync();

                    // Add a non query command
                    context.Database.ExecuteSqlRaw("INSERT INTO Users (Id) VALUES ({0})", 5);

                    // Add a non query async command
                    await context.Database.ExecuteSqlRawAsync("INSERT INTO Users (Id) VALUES ({0})", 6);

                    // TODO: missing coverage for the ScalarExecutedAsync method in the interceptor.  I don't know how to force EF to call that
                }

                var results = httpContextAccessorMock.Object.HttpContext.Items;

                results.TryGetValue(DatabaseAnalyticsInterceptor.TRANSACTION_TIME_TOKEN, out var transactionTime).ShouldBe(true);
                results.TryGetValue(DatabaseAnalyticsInterceptor.TRANSACTION_COUNT_TOKEN, out var transactionCount).ShouldBe(true);

                ((double)transactionTime).ShouldBeGreaterThan(0);
                ((int)transactionCount).ShouldBe(7);

            }
            finally
            {
                connection.Close();
            }
        } 

        private DbContextOptions<TestDbContext> SetupInMemoryDatabase(IHttpContextAccessor httpContextAccessor, out SqliteConnection connection)
        {
            // In-memory database only exists while the connection is open
            connection = new SqliteConnection("DataSource=:memory:");

            return new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite(connection)
                    .AddInterceptors(new DatabaseAnalyticsInterceptor(httpContextAccessor))
                    .Options;
        }

        private void CreateInMemoryDatabase(DbContextOptions<TestDbContext> databaseOptions)
        {
            using (var context = new TestDbContext(databaseOptions))
            {
                context.Database.EnsureCreated();
            }
        }
    }

    internal class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

    }

    internal class User
    {
        public int Id { get; set; }
    }
}
