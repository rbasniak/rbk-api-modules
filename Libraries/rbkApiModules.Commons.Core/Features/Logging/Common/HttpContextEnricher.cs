using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Core.Logging;

public class HttpContextEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return;
        } 

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Method", httpContext.Request.Method, false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Path", httpContext.Request.Path, false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Response", httpContext.Response.StatusCode, false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("PathBase", httpContext.Request.PathBase, false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("IpAddress", httpContext.Connection.RemoteIpAddress.ToString(), false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ConnectionId", httpContext.Connection.Id, false));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", httpContext.User.Identity.Name, false));
    }
}
public class HttpContextModel
{
    public string Method { get; init; }
    public string Path { get; init; }
}