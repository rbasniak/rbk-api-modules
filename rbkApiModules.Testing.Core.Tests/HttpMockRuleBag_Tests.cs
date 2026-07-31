using System.Net;
using System.Text;
using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class HttpMockRuleBag_Tests
{
    private interface IExternalClient;

    [Test]
    public void FindMatch_LastRegisteredMatchingRuleWins()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, urlMatcher: null, "first"));
        bag.Add(Rule(HttpMethod.Get, url => url.Contains("doc.pdf"), "second"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/doc.pdf");

        bag.FindMatch(typeof(IExternalClient), request)!.ResponseFactory().Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("second");
    }

    [Test]
    public void FindMatch_LastRegisteredAnyUrlRuleWins()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, urlMatcher: null, "first"));
        bag.Add(Rule(HttpMethod.Get, urlMatcher: null, "second"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/anything");

        bag.FindMatch(typeof(IExternalClient), request)!.ResponseFactory().Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("second");
    }

    [Test]
    public void FindMatch_UsesUrlMatcherOnFullRequestUrl()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, url => url.Contains("/api/files/1"), "matched"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/files/1");

        bag.FindMatch(typeof(IExternalClient), request).ShouldNotBeNull();
    }

    [Test]
    public void FindMatch_ReturnsNullWhenMethodDoesNotMatch()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Post, urlMatcher: null, "post"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");

        bag.FindMatch(typeof(IExternalClient), request).ShouldBeNull();
    }

    [Test]
    public void FindMatch_UsesUrlPredicateWhenProvided()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(
            HttpMethod.Get,
            url => url.Contains("target-doc", StringComparison.OrdinalIgnoreCase),
            "matched"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/files/target-doc.pdf");

        bag.FindMatch(typeof(IExternalClient), request)!.ResponseFactory().Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("matched");
    }

    private static HttpMockRule Rule(HttpMethod method, Func<string, bool>? urlMatcher, string body) =>
        new()
        {
            ClientType = typeof(IExternalClient),
            Method = method,
            UrlMatcher = urlMatcher,
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "text/plain")
            }
        };
}
