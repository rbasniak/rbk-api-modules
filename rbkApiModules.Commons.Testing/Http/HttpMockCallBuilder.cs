using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace rbkApiModules.Commons.Testing;

public sealed class HttpMockCallBuilder
{
    private readonly Type _clientType;
    private readonly HttpMethod _method;
    private readonly Func<string, bool>? _urlMatcher;

    internal HttpMockCallBuilder(Type clientType, HttpMethod method, Func<string, bool>? urlMatcher)
    {
        _clientType = clientType;
        _method = method;
        _urlMatcher = urlMatcher;
    }

    public void ReturnsSuccess(HttpContent content) =>
        Returns(HttpStatusCode.OK, content);

    public void ReturnsSuccess(byte[] content, string? mediaType = null) =>
        Returns(HttpStatusCode.OK, CreateByteContent(content, mediaType));

    public void ReturnsSuccess(string content, string? mediaType = "application/json") =>
        Returns(HttpStatusCode.OK, CreateStringContent(content, mediaType));

    public void ReturnsBadRequest(HttpContent content) =>
        Returns(HttpStatusCode.BadRequest, content);

    public void ReturnsBadRequest(byte[] content, string? mediaType = null) =>
        Returns(HttpStatusCode.BadRequest, CreateByteContent(content, mediaType));

    public void ReturnsBadRequest(string content, string? mediaType = "application/json") =>
        Returns(HttpStatusCode.BadRequest, CreateStringContent(content, mediaType));

    public void ReturnsUnauthorized() =>
        Returns(HttpStatusCode.Unauthorized, new StringContent(string.Empty));

    public void ReturnsUnauthorized(HttpContent content) =>
        Returns(HttpStatusCode.Unauthorized, content);

    public void ReturnsUnauthorized(string content, string? mediaType = "application/json") =>
        Returns(HttpStatusCode.Unauthorized, CreateStringContent(content, mediaType));

    public void Returns(HttpStatusCode statusCode, HttpContent content)
    {
        var rules = HttpMockScope.CurrentRules
            ?? throw new InvalidOperationException(
                $"No {nameof(HttpMockScope)} is active. Call {nameof(HttpMockScope)}.{nameof(HttpMockScope.Begin)}() " +
                $"(or TestingServer.HttpMockScope()) before configuring mock responses.");

        // Capture content bytes now so each invocation gets a fresh HttpContent instance.
        var payload = content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
        var contentType = content.Headers.ContentType?.ToString();

        rules.Add(new HttpMockRule
        {
            ClientType = _clientType,
            Method = _method,
            UrlMatcher = _urlMatcher,
            ResponseFactory = () =>
            {
                var responseContent = new ByteArrayContent(payload);
                if (!string.IsNullOrEmpty(contentType))
                {
                    responseContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                }

                return new HttpResponseMessage(statusCode)
                {
                    Content = responseContent
                };
            }
        });
    }

    public void Returns(HttpStatusCode statusCode, byte[] content, string? mediaType = null) =>
        Returns(statusCode, CreateByteContent(content, mediaType));

    public void Returns(HttpStatusCode statusCode, string content, string? mediaType = "application/json") =>
        Returns(statusCode, CreateStringContent(content, mediaType));

    private static ByteArrayContent CreateByteContent(byte[] content, string? mediaType)
    {
        var httpContent = new ByteArrayContent(content);
        if (!string.IsNullOrEmpty(mediaType))
        {
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        }

        return httpContent;
    }

    private static StringContent CreateStringContent(string content, string? mediaType) =>
        new(content, Encoding.UTF8, mediaType ?? "application/json");
}
