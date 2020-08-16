using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// Middleware que salva os dados de cada request no banco
    /// </summary>
    public class AnalyticBuilder
    {
        private readonly IAnalyticStore _store;
        private List<Func<HttpContext, bool>> _exclude;

        internal AnalyticBuilder(IAnalyticStore store)
        {
            _store = store;
        }

        internal async Task Run(HttpContext context, Func<Task> next)
        {
            var stopwatch = Stopwatch.StartNew();

            await next.Invoke();

            //This request should be filtered out ?
            if (_exclude?.Any(x => x(context)) ?? false)
            {
                return;
            }

            stopwatch.Stop();

            try
            {
                var identity = context.UserIdentity();

                var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
                var username = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

                var req = new WebRequest
                {
                    Timestamp = DateTime.UtcNow,
                    Identity = identity,
                    Username = username,
                    RemoteIpAddress = context.Connection.RemoteIpAddress,
                    Method = context.Request.Method,
                    UserAgent = context.Request.Headers["User-Agent"],
                    IsHttps = context.Request.IsHttps,
                    Path = context.Request.Path.Value,
                    Response = context.Response.StatusCode,
                    Duration = stopwatch.ElapsedMilliseconds, 
                };

                Debug.WriteLine($"{req.Method} {req.Path} [{stopwatch.ElapsedMilliseconds}ms]");

                //Store the request into the store
                _ = _store.StoreWebRequestAsync(req);
            }
            catch
            {

            }
        }

        public AnalyticBuilder Exclude(Func<HttpContext, bool> filter)
        {
            if (_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public AnalyticBuilder Exclude(IPAddress ip) => Exclude(x => Equals(x.Connection.RemoteIpAddress, ip));

        public AnalyticBuilder LimitToPath(string path) => Exclude(x => !x.Request.Path.StartsWithSegments(path));

        public AnalyticBuilder ExcludePath(params string[] paths)
        {
            return Exclude(x => paths.Any(path => x.Request.Path.StartsWithSegments(path)));
        }

        public AnalyticBuilder ExcludeExtension(params string[] extensions)
        {
            return Exclude(x => extensions.Any(ext => x.Request.Path.Value.EndsWith(ext)));
        }

        public AnalyticBuilder ExcludeMethod(params string[] methods)
        {
            return Exclude(x => methods.Any(ext => x.Request.Method == "OPTIONS"));
        }

        public AnalyticBuilder ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public AnalyticBuilder ExcludeIp(IPAddress address) => Exclude(x => x.Connection.RemoteIpAddress.Equals(address));

        public AnalyticBuilder ExcludeStatusCodes(params HttpStatusCode[] codes) => Exclude(context => codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticBuilder ExcludeStatusCodes(params int[] codes) => Exclude(context => codes.Contains(context.Response.StatusCode));

        public AnalyticBuilder LimitToStatusCodes(params HttpStatusCode[] codes) => Exclude(context => !codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticBuilder LimitToStatusCodes(params int[] codes) => Exclude(context => !codes.Contains(context.Response.StatusCode));
    }
}
