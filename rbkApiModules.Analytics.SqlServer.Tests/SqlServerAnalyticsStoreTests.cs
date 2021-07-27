using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;

namespace rbkApiModules.Analytics.SqlServer.Tests
{
    public class SqlServerAnalyticsStoreTests
    {
        private DbContextOptions<SqlServerAnalyticsContext> SetupInMemoryDatabase(out SqliteConnection connection)
        {
            // In-memory database only exists while the connection is open
            connection = new SqliteConnection("DataSource=:memory:");

            return new DbContextOptionsBuilder<SqlServerAnalyticsContext>()
                    .UseSqlite(connection)
                    .Options;
        }

        private void CreateInMemoryDatabase(DbContextOptions<SqlServerAnalyticsContext> databaseOptions)
        {
            using (var context = new SqlServerAnalyticsContext(databaseOptions))
            {
                context.Database.EnsureCreated();
            }
        }

        [AutoNamedFact]
        public void Should_Save_Analytics_Data()
        {
            var data = new AnalyticsEntry 
            {
                Action = "/api/details/{id}",
                Area = "Selling",
                Domain = "Company A",
                Duration = 19,
                Identity = "156456465651614",
                IpAddress = "192.168.0.1",
                Method = "GET",
                Path = "/api/details/59",
                RequestSize = 21,
                Response = 204,
                ResponseSize = 29,
                Timestamp = new DateTime(2020, 02, 03, 04, 05, 06),
                TotalTransactionTime = 31,
                TransactionCount = 7,
                UserAgent = "Mozilla",
                Username = "admin",
                Version = "1.0.8",
                WasCached = true
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    store.StoreData(data);
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var savedData = context.Data.First();

                    savedData.Action.ShouldBe(data.Action);
                    savedData.Area.ShouldBe(data.Area);
                    savedData.Domain.ShouldBe(data.Domain);
                    savedData.Duration.ShouldBe(data.Duration);
                    savedData.Identity.ShouldBe(data.Identity);
                    savedData.IpAddress.ShouldBe(data.IpAddress);
                    savedData.Method.ShouldBe(data.Method);
                    savedData.Path.ShouldBe(data.Path);
                    savedData.RequestSize.ShouldBe(data.RequestSize);
                    savedData.Response.ShouldBe(data.Response);
                    savedData.ResponseSize.ShouldBe(data.ResponseSize);
                    savedData.Timestamp.ShouldBe(data.Timestamp);
                    savedData.TotalTransactionTime.ShouldBe(data.TotalTransactionTime);
                    savedData.TransactionCount.ShouldBe(data.TransactionCount);
                    savedData.UserAgent.ShouldBe(data.UserAgent);
                    savedData.Version.ShouldBe(data.Version);
                    savedData.WasCached.ShouldBe(data.WasCached);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Time_Interval()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Identity = "1", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Identity = "2", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Identity = "3", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Identity = "4", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Identity = "5", Timestamp = new DateTime(2020, 01, 03, 00, 00, 00) },
                new AnalyticsEntry { Identity = "6", Timestamp = new DateTime(2020, 01, 04, 00, 00, 00) },
                new AnalyticsEntry { Identity = "7", Timestamp = new DateTime(2020, 01, 05, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 02), new DateTime(2020, 01, 04));
                    
                    // Devem haver somente 4 entradas entre o dia 2 e o dia 4
                    filteredSaveData.Count.ShouldBe(4);
                    filteredSaveData.SingleOrDefault(x => x.Identity == "3").ShouldNotBeNull();
                    filteredSaveData.SingleOrDefault(x => x.Identity == "4").ShouldNotBeNull();
                    filteredSaveData.SingleOrDefault(x => x.Identity == "5").ShouldNotBeNull();
                    filteredSaveData.SingleOrDefault(x => x.Identity == "6").ShouldNotBeNull();

                    // Aproveitando a mesma infraestrutura do teste para testar o .AllAsync()
                    var allSavedData = await store.GetStatisticsAsync();

                    allSavedData.Count.ShouldBe(7);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Version()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Version = "1.0.0", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Version = "1.0.0", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Version = "2.0.0", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Version = "3.0.0", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), new[] { "1.0.0", "3.0.0" },
                        null, null, null, null, null, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Version == "1.0.0").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Version == "3.0.0").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Area()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Area = "clients", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Area = "clients", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Area = "orders", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Area = "analytics", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                        new[] { "clients", "orders" }, null, null, null, null, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Area == "clients").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Area == "orders").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Domain()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Domain = "domain1", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Domain = "domain1", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Domain = "domain2", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Domain = "domain3", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                        null, new[] { "domain1", "domain2" }, null, null, null, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Domain == "domain1").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Domain == "domain2").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Action()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Action = "api/action/{id}", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Action = "api/action/{id}", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Action = "api/action", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Action = "api/action/create", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                        null, null, new[] { "api/action/{id}", "api/action" }, null, null, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Action == "api/action/{id}").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Action == "api/action").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_User()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Username = "admin", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Username = "admin", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Username = "viewer", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Username = "seller", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                                            null, null, null, new[] { "admin", "viewer" }, null, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Username == "admin").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Username == "viewer").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Agent()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { UserAgent = "chrome", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { UserAgent = "chrome", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { UserAgent = "firefox", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { UserAgent = "opera", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                                            null, null, null, null, new[] { "chrome", "firefox" }, null, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.UserAgent == "chrome").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.UserAgent == "firefox").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Response()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Response = 200, Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Response = 200, Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Response = 500, Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Response = 404, Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                                            null, null, null, null, null, new[] { 200, 500 }, null, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Response == 200).Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Response == 500).Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_Method()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Method = "GET", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Method = "GET", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Method = "POST", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Method = "PUT", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                         null, null, null, null, null, null, new[] { "GET", "PUT" }, 0, null);

                    filteredSaveData.Count.ShouldBe(3);
                    filteredSaveData.Where(x => x.Method == "GET").Count().ShouldBe(2);
                    filteredSaveData.Where(x => x.Method == "PUT").Count().ShouldBe(1);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [AutoNamedFact]
        public async void Should_Filter_Analytics_Data_By_EntityId()
        {
            var data = new List<AnalyticsEntry>
            {
                new AnalyticsEntry { Path = "api/users/2", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Path = "api/users/2", Timestamp = new DateTime(2020, 01, 01, 00, 00, 00) },
                new AnalyticsEntry { Path = "api/users/3", Timestamp = new DateTime(2020, 01, 02, 00, 00, 00) },
                new AnalyticsEntry { Path = "api/users/4", Timestamp = new DateTime(2020, 01, 15, 00, 00, 00) },
            };

            var databaseOptions = SetupInMemoryDatabase(out var connection);

            try
            {
                connection.Open();

                CreateInMemoryDatabase(databaseOptions);

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    context.AddRange(data);
                    context.SaveChanges();
                }

                using (var context = new SqlServerAnalyticsContext(databaseOptions))
                {
                    var store = new SqlServerAnalyticStore(context, MockOptions().Object);
                    var filteredSaveData = await store.FilterStatisticsAsync(new DateTime(2020, 01, 01), new DateTime(2020, 01, 30), null,
                        null, null, null, null, null, null, null, 0, "2");

                    filteredSaveData.Count.ShouldBe(2);
                    filteredSaveData.Where(x => x.Path == "api/users/2").Count().ShouldBe(2); 
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private Mock<IOptions<RbkAnalyticsModuleOptions>> MockOptions()
        {
            var mock = new Mock<IOptions<RbkAnalyticsModuleOptions>>();
            mock.Setup(x => x.Value.TimezoneOffsetHours).Returns(0);
            
            return mock;
        }
    } 
}
