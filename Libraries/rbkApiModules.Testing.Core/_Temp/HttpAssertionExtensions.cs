using Shouldly;
using System.Net;

namespace rbkApiModules.Testing.Core;

public static class HttpAssertionExtensions
{
    public static void ShouldHaveErrors(this HttpResponse response, HttpStatusCode expectedCode, params string[] messages)
    {
        response.Code.ShouldBe(expectedCode, $"Unexpected http response code. Should be {expectedCode} but was {response.Code}");

        response.Messages.Length.ShouldBe(messages.Length, $"Unexpected number of error messages. Should be {messages.Length} but was {response.Messages.Length}");
        
        foreach (var message in messages)
        {
            response.Messages.ShouldContain(message, $"Could not find the [{message}] message in the response");
        }
    }

    public static void ShouldHaveErrors<T>(this HttpResponse<T> response, HttpStatusCode expectedCode, params string[] messages) where T : class
    {
        response.Code.ShouldBe(expectedCode);

        response.Data.ShouldBeNull();
        response.Messages.Length.ShouldBe(messages.Length);
        
        foreach (var message in messages)
        {
            response.Messages.ShouldContain(message);
        }
    }

    public static void ShouldRedirect(this HttpResponse response, string url)
    {
        response.Code.ShouldBe(HttpStatusCode.Redirect);

        response.Messages.Length.ShouldBe(1);

        response.Messages[0].ShouldBe(url);
    }

    public static T ShouldBeSuccess<T>(this HttpResponse<T> response) where T : class
    {
        response.Code.ShouldBe(HttpStatusCode.OK, String.Join(", ", response.Messages));
        response.Data.ShouldNotBeNull($"Expected response of type {typeof(T).Name}, but the response was empty");
        response.Data.ShouldBeOfType<T>($"Expected response of type {typeof(T).Name}, but the response was of type {response.Data.GetType().Name}");

        return response.Data;
    }

    public static void ShouldBeSuccess<T>(this HttpResponse<T> response, out T result) where T : class
    {
        response.Code.ShouldBe(HttpStatusCode.OK, String.Join(", ", response.Messages));
        response.Data.ShouldNotBeNull($"Expected response of type {typeof(T).Name}, but the response was empty");
        response.Data.ShouldBeOfType<T>($"Expected response of type {typeof(T).Name}, but the response was of type {response.Data.GetType().Name}");

        result = response.Data;
    }

    public static void ShouldBeSuccess(this HttpResponse response)
    {
        response.Code.ShouldBe(HttpStatusCode.OK);
    }

    public static void ShouldBeForbidden(this HttpResponse response)
    {
        response.Code.ShouldBe(HttpStatusCode.Forbidden);
    }
}
