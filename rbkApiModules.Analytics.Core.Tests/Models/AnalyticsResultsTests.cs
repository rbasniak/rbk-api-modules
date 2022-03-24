using rbkApiModules.Utilities.Testing;
using Xunit;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class AnalyticsResultsTests
    {
        [AutoNamedFact]
        [Trait(TraitTokens.MODELS, nameof(AnalyticsDashboard))]
        public void Should_Initialize_All_Lists()
        {
            var instance = new AnalyticsDashboard();

            instance.ShouldHaveAllListInitialized();
        }
    }
}
