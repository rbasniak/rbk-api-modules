using System.Net;

namespace rbkApiModules.Testing.Core;

public class HttpResponse
{
    public HttpResponse()
    {
        Messages = new string[0];
    }

    public HttpStatusCode Code { get; set; }
    public string[] Messages { get; set; }

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

    public T Data { get; set; }

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