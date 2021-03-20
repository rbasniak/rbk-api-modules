using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class FilterAnalyticsEntriesTests
    {
        [AutoNamedFact]
        public async void Should_Return_Analytics_Entries()
        {
            var storeMock = new Mock<IAnalyticModuleStore>();

            storeMock.Setup(x => x.FilterAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>(),
                It.IsAny<int[]>(),
                It.IsAny<string[]>(),
                It.IsAny<int>(),
                It.IsAny<string>())).Returns(Task.FromResult(new List<AnalyticsEntry> {
                    new AnalyticsEntry(),
                    new AnalyticsEntry(),
                    new AnalyticsEntry(),
                }));

            var command = new FilterAnalyticsEntries.Command
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now,
                Actions = new string[0],
                Areas = new string[0],
                Domains = new string[0],
                Responses = new int[0],
                Users = new string[0],
                Agents = new string[0],
                Versions = new string[0],
                Duration = 0,
                EntityId = null
            };

            var httpContextMock = new Mock<IHttpContextAccessor>();

            var handler = new FilterAnalyticsEntries.Handler(httpContextMock.Object, storeMock.Object);

            var response = await handler.Handle(command, default);

            response.IsValid.ShouldBeTrue();
            response.Result.ShouldNotBeNull();

            var results = response.Result.ShouldBeOfType<List<AnalyticsEntry>>();

            results.Count.ShouldBe(3);
        }
    }
}
