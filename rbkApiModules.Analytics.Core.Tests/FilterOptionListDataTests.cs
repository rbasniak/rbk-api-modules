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
    public class FilterOptionListDataTests 
    {
        [AutoNamedFact]
        [Trait(TraitTokens.MODELS, nameof(FilterOptionListData))]
        public void Should_Initialize_All_Lists()
        {
            var instance = new FilterOptionListData();

            var tester = new ListInitializationTester(instance);
            var results = tester.Test();

            results.ShouldNotBe(null);
            results.Count.ShouldBe(0, "Non initialized lists: " + String.Join(", ", results));
        }
    }
}
