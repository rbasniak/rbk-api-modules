using Microsoft.AspNetCore.Http;
using Moq;
using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class GetDashboardDataTests
    {
        [AutoNamedFact]
        public async void Should_Return_Dashboard_Data()
        {
            var storeMock = new Mock<IAnalyticModuleStore>();

            var random = new Random(1901);

            var versions = new[] { "1.0.0", "1.1.3" };
            var methods = new[] { "GET", "POST", "PUT", "DELETE" };
            var responses = new[] { 200, 204, 400, 500, 401 };

            var analytics = new List<AnalyticsEntry>();

            for (int i = 0; i < 30; i++)
            {
                var version = versions[random.Next(0, versions.Length)];
                var area = "Area " + random.Next(0, 2);
                var timestamp = new DateTime(2020, 01, 01).AddDays(random.Next(0, 3));
                var username = "User" + random.Next(0, 2);
                var domain = "Domain " + random.Next(0, 2);
                var agent = "UserAgent " + random.Next(0, 2);
                var method = methods[random.Next(0, methods.Length)];
                var httpResponse = responses[random.Next(0, responses.Length)];
                var path = "/api/path/" + random.Next(0, 2);
                var action = "/api/action/{id}";
                var responseSize = random.Next(5, 25);
                var requestSize = random.Next(10, 30);
                var duration = random.Next(50, 75);
                var transactionTime = random.Next(35, 45);
                var transactionCount = random.Next(0, 3);

                analytics.Add(new AnalyticsEntry
                {
                    Action = action,
                    Area = area,
                    Domain = domain,
                    Duration = duration,
                    Method = method,
                    Path = path,
                    RequestSize = requestSize,
                    Response = httpResponse,
                    ResponseSize = responseSize,
                    Timestamp = timestamp,
                    TotalTransactionTime = transactionTime,
                    TransactionCount = transactionCount,
                    UserAgent = agent,
                    Username = username,
                    Version = version,
                });
            }

            // Debug.WriteLine(JsonConvert.SerializeObject(analytics));

            storeMock.Setup(x => x.FilterStatisticsAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>())).Returns(Task.FromResult(analytics));

            var command = new GetDashboardData.Command
            {
                DateFrom = new DateTime(2020, 01, 01),
                DateTo = new DateTime(2020, 01, 01).AddDays(3)
            };

            var httpContextMock = new Mock<IHttpContextAccessor>();

            var handler = new GetDashboardData.Handler(httpContextMock.Object, storeMock.Object);

            var response = await handler.Handle(command, default);

            response.IsValid.ShouldBeTrue();
            response.Result.ShouldNotBeNull();

            response.Result.ShouldBeOfType<AnalyticsDashboard>();
        }
    }
}
