using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class HttpMockScope_Tests
{
    [Test]
    public void Begin_CreatesActiveScope()
    {
        using var scope = HttpMockScope.Begin();

        HttpMockScope.CurrentRules.ShouldNotBeNull();
        HttpMockScope.CurrentRules.ShouldBeSameAs(scope.Rules);
    }

    [Test]
    public void Dispose_RestoresPreviousScope()
    {
        using var outer = HttpMockScope.Begin();
        var outerRules = outer.Rules;

        using (var inner = HttpMockScope.Begin())
        {
            HttpMockScope.CurrentRules.ShouldBeSameAs(inner.Rules);
            HttpMockScope.CurrentRules.ShouldNotBeSameAs(outerRules);
        }

        HttpMockScope.CurrentRules.ShouldBeSameAs(outerRules);
    }

    [Test]
    public void Dispose_ClearsScopeWhenNoParentExists()
    {
        using var scope = HttpMockScope.Begin();
        scope.Rules.ShouldNotBeNull();

        scope.Dispose();

        HttpMockScope.CurrentRules.ShouldBeNull();
    }
}
