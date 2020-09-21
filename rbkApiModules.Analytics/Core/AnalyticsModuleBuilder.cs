using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.AspNetCore.Builder;
using System.IO;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsModuleMiddleware
    {
        private List<Func<HttpContext, bool>> _exclude;
        private readonly RequestDelegate _next;
        public AnalyticsModuleMiddleware(RequestDelegate next, AnalyticsModuleOptions options)
        {
            _next = next;
            _exclude = options.ExcludeRules;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            context.Request.EnableBuffering();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                var identity = UserIdentity(context);

                var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
                var username = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var domain = user.Claims.FirstOrDefault(c => c.Type == "domain")?.Value;

                // TODO: get version from somewhere
                var data = new AnalyticsEntry("1.0.0", "", identity, username, domain, context.Connection.RemoteIpAddress.ToString(),
                    context.Request.Headers["User-Agent"], context.Request.Method + " " + context.Request.Path, context.Request.Method, "", -1, -1, -1, -1);

                //new MemoryStream. 
                using (var responseBody = new MemoryStream())
                {
                    // temporary response body 
                    context.Response.Body = responseBody;
                    //execute the Middleware pipeline 
                    await _next(context);
                    //read the response stream from the beginning
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    //Copy the contents of the new memory stream
                    await responseBody.CopyToAsync(originalBodyStream);

                    var responseSize = 0L;
                    if(context.Response.Body.CanSeek && context.Response.Body.CanRead)
                    {
                        responseSize = context.Response.Body.Length;
                    }

                    var requestSize = 0L;
                    if (context.Request.Body.CanSeek && context.Request.Body.CanRead)
                    {
                        requestSize = context.Request.Body.Length;
                    }

                    stopwatch.Stop();

                    if (!_exclude?.Any(x => x(context)) ?? true)
                    {
                        var areaData = context.Items.FirstOrDefault(x => x.Key.ToString() == "log-data-area");
                        var pathData = context.Items.FirstOrDefault(x => x.Key.ToString() == "log-data-path");

                        if (areaData.Key != null)
                        {
                            data.Area = areaData.Value as string;
                        }

                        if (pathData.Key != null)
                        {
                            data.Action = context.Request.Method + " " + pathData.Value as string;
                        }

                        data.Duration = (int)stopwatch.ElapsedMilliseconds;
                        data.Response = context.Response.StatusCode;
                        data.RequestSize = requestSize;
                        data.ResponseSize = responseSize;

                        var store = context.RequestServices.GetService<IAnalyticModuleStore>();

                        store.StoreData(data);
                    }
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private string UserIdentity(HttpContext context)
        {
            var cookieName = "SSA_Identity";

            string identity = context.User?.Identity?.Name;

            if (!context.Request.Cookies.ContainsKey(cookieName))
            {
                if (string.IsNullOrWhiteSpace(identity))
                {
                    identity = context.Request.Cookies.ContainsKey("ai_user")
                                ? context.Request.Cookies["ai_user"]
                                : context.Connection.Id;
                }

                if (!context.Response.HasStarted)
                {
                    context.Response.Cookies.Append("identity", identity);
                }
            }
            else
            {
                identity = context.Request.Cookies[cookieName];
            }

            return identity;
        }
    }

    public class AnalyticsModuleOptions
    {
        private List<Func<HttpContext, bool>> _exclude;

        public AnalyticsModuleOptions Exclude(Func<HttpContext, bool> filter)
        {
            if (_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public List<Func<HttpContext, bool>> ExcludeRules => _exclude;

        public AnalyticsModuleOptions Exclude(IPAddress ip) => Exclude(x => Equals(x.Connection.RemoteIpAddress, ip));

        public AnalyticsModuleOptions LimitToPath(string path) => Exclude(x => !x.Request.Path.StartsWithSegments(path));

        public AnalyticsModuleOptions ExcludePath(params string[] paths)
        {
            return Exclude(x => paths.Any(path => x.Request.Path.StartsWithSegments(path)));
        }

        public AnalyticsModuleOptions ExcludeExtension(params string[] extensions)
        {
            return Exclude(x => extensions.Any(ext => x.Request.Path.Value.EndsWith(ext)));
        }

        public AnalyticsModuleOptions ExcludeMethods(params string[] methods)
        {
            return Exclude(x => methods.Any(ext => x.Request.Method == ext));
        }

        public AnalyticsModuleOptions ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public AnalyticsModuleOptions ExcludeIp(IPAddress address) => Exclude(x => x.Connection.RemoteIpAddress.Equals(address));

        public AnalyticsModuleOptions ExcludeStatusCodes(params HttpStatusCode[] codes) => Exclude(context => codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleOptions ExcludeStatusCodes(params int[] codes) => Exclude(context => codes.Contains(context.Response.StatusCode));

        public AnalyticsModuleOptions LimitToStatusCodes(params HttpStatusCode[] codes) => Exclude(context => !codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleOptions LimitToStatusCodes(params int[] codes) => Exclude(context => !codes.Contains(context.Response.StatusCode));
    }
}
