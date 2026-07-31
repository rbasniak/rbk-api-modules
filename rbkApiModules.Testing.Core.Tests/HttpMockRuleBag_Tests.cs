using System.Net;
using System.Text;
using Shouldly;

namespace rbkApiModules.Testing.Core.Tests;

public class HttpMockRuleBag_Tests
{
    private interface IExternalClient;

    [Test]
    public void FindMatch_PrefersSpecificUrlOverAnyUrl()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, url: null, "any"));
        bag.Add(Rule(HttpMethod.Get, "http://localhost/doc.pdf", "specific"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/doc.pdf");

        bag.FindMatch(typeof(IExternalClient), request)!.ResponseFactory().Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("specific");
    }

    [Test]
    public void FindMatch_LastRegisteredAnyUrlRuleWins()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, url: null, "first"));
        bag.Add(Rule(HttpMethod.Get, url: null, "second"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/anything");

        bag.FindMatch(typeof(IExternalClient), request)!.ResponseFactory().Content!
            .ReadAsStringAsync().GetAwaiter().GetResult().ShouldBe("second");
    }

    [Test]
    public void FindMatch_MatchesPathAndQueryWhenRuleUsesRelativeUrl()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Get, "/api/files/1", "matched"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/files/1");

        bag.FindMatch(typeof(IExternalClient), request).ShouldNotBeNull();
    }

    [Test]
    public void FindMatch_ReturnsNullWhenMethodDoesNotMatch()
    {
        var bag = new HttpMockRuleBag();
        bag.Add(Rule(HttpMethod.Post, url: null, "post"));

        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/");

        bag.FindMatch(typeof(IExternalClient), request).ShouldBeNull();
    }

    private static HttpMockRule Rule(HttpMethod method, string? url, string body) =>
        new()
        {
            ClientType = typeof(IExternalClient),
            Method = method,
            Url = url,
            ResponseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "text/plain")
            }
        };
}
