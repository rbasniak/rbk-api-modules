using rbkApiModules.Utilities.Testing;
using Xunit;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class AnalyticsEnttryTests
    {
        [AutoNamedFact]
        [Trait(TraitTokens.MODELS, nameof(AnalyticsEntry))]
        public void Should_Initialize_All_Lists()
        {
            var instance = new AnalyticsEntry();

            instance.ShouldHaveAllListInitialized();
        }
    }
}
