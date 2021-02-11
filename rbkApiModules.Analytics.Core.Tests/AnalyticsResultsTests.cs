using rbkApiModules.Utilities.Testing;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace rbkApiModules.Analytics.Core.Tests
{
    public class AnalyticsResultsTests
    {
        [AutoNamedFact]
        [Trait(TraitTokens.MODELS, nameof(AnalyticsResults))]
        public void Should_Initialize_All_Lists()
        {
            var instance = new AnalyticsResults();

            instance.ShouldHaveAllListInitialized();
        }
    }
}
