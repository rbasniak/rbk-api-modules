using rbkApiModules.Utilities.Testing;
using Shouldly;
using Xunit;

namespace rbkApiModules.Analytics.Core.Tests
{

    public class TransactionCounterTests
    {
        [AutoNamedFact()]
        [Trait(TraitTokens.MODELS, nameof(TransactionCounter))]
        public void Getters_And_Setters_Should_Work()
        {
            var transactionCounter = new TransactionCounter();
            transactionCounter.TotalTime = 99;
            transactionCounter.Transactions = 15;

            transactionCounter.TotalTime.ShouldBe(99);
            transactionCounter.Transactions.ShouldBe(15);
        }
    }
}
