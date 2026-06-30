using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace rbkApiModules.Commons.Testing;

public class HttpResponse
{
    public HttpResponse()
    {
        Messages = [];
        RequestHeaders = new Dictionary<string, string[]>();
        ResponseHeaders = new Dictionary<string, string[]>();
    }

    public HttpStatusCode Code { get; set; }
    public string[] Messages { get; set; }
    public string Body { get; set; }
    public ProblemDetails? Problem { get; set; }
    public IReadOnlyDictionary<string, string[]> RequestHeaders { get; set; }
    public IReadOnlyDictionary<string, string[]> ResponseHeaders { get; set; }
    public bool IsSuccess => Code == HttpStatusCode.OK || Code == HttpStatusCode.NoContent;

    public override string ToString()
    {
        var messages = String.Empty;

        if (Messages.Length > 0)
        {
            messages = String.Join(", ", Messages);
        }

        return $"[{(int)Code}] {messages}";
    }
}

public class HttpResponse<T> : HttpResponse where T : class
{
    public HttpResponse() : base()
    {
        Data = null;
    }

    public T? Data { get; set; }

    public override string ToString()
    {
        var messages = String.Empty;

        if (Messages.Length > 0)
        {
            messages = String.Join(", ", Messages);
        }

        var data = String.Empty;
        if (Data != null)
        {
            data = Data.GetType().Name;
        }

        return $"[{(int)Code}] {messages}{data}";
    }
}