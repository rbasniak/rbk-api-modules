using Microsoft.AspNetCore.Http;
using Moq;
using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class GetFilteringListsTests
    {
        [AutoNamedFact]
        public async void Should_Return_Lists_For_Filtering()
        {
            var storeMock = new Mock<IAnalyticModuleStore>();

            var random = new Random(1901);

            var versions = new[] { "1.0.0", "1.1.3" };
            var methods = new[] { "GET", "POST", "PUT", "DELETE" };
            var responses = new[] { 200, 204, 400, 500, 401 };

            var analytics = new List<AnalyticsEntry>();

            for (int i = 0; i < 50; i++)
            {
                var version = versions[random.Next(0, versions.Length)];
                var area = "Area " + random.Next(0, 3);
                var username = "User" + random.Next(0, 3);
                var domain = "Domain " + random.Next(0, 6);
                var agent = "UserAgent " + random.Next(0, 2);
                var httpResponse = responses[random.Next(0, responses.Length)];
                var action = "/api/action/{id}";

                analytics.Add(new AnalyticsEntry
                {
                    Action = action,
                    Area = area,
                    Domain = domain,
                    Response = httpResponse,
                    UserAgent = agent,
                    Username = username,
                    Version = version,
                });
            }

            storeMock.Setup(x => x.GetStatisticsAsync()).Returns(Task.FromResult(analytics));

            var command = new GetFilteringLists.Command();

            var httpContextMock = new Mock<IHttpContextAccessor>();

            var handler = new GetFilteringLists.Handler(httpContextMock.Object, storeMock.Object);

            var response = await handler.Handle(command, default);

            response.IsValid.ShouldBeTrue();
            response.Result.ShouldNotBeNull();

            var result = response.Result.ShouldBeOfType<FilterOptionListData>();

            result.Actions.Count.ShouldBe(1);
            result.Agents.Count.ShouldBe(2);
            result.Areas.Count.ShouldBe(3);
            result.Domains.Count.ShouldBe(6);
            result.Responses.Count.ShouldBe(5);
            result.Users.Count.ShouldBe(3);
            result.Versions.Count.ShouldBe(2);
        }
    }
}
