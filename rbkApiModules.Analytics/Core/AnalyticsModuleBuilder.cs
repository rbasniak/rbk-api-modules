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

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsModuleBuilder
    {
        private List<Func<HttpContext, bool>> _exclude;

        public AnalyticsModuleBuilder()
        {
        }

        public async Task Run(HttpContext context, Func<Task> next)
        {
            var stopwatch = Stopwatch.StartNew();

            var identity = UserIdentity(context);

            var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
            var username = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var domain = user.Claims.FirstOrDefault(c => c.Type == "domain")?.Value;

            // TODO: get version from somewhere
            var data = new AnalyticsEntry("1.0.0", "", identity, username, domain, context.Connection.RemoteIpAddress.ToString(),
                context.Request.Headers["User-Agent"], context.Request.Method + " " + context.Request.Path, context.Request.Method, "", -1, -1);

            await next.Invoke();

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
                    data.Action = context.Request.Method + " " +  pathData.Value as string;
                }

                data.Duration = (int)stopwatch.ElapsedMilliseconds;
                data.Response = context.Response.StatusCode;

                var store = context.RequestServices.GetService<IAnalyticModuleStore>();

                store.StoreData(data);
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

        public AnalyticsModuleBuilder Exclude(Func<HttpContext, bool> filter)
        {
            if (_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public AnalyticsModuleBuilder Exclude(IPAddress ip) => Exclude(x => Equals(x.Connection.RemoteIpAddress, ip));

        public AnalyticsModuleBuilder LimitToPath(string path) => Exclude(x => !x.Request.Path.StartsWithSegments(path));

        public AnalyticsModuleBuilder ExcludePath(params string[] paths)
        {
            return Exclude(x => paths.Any(path => x.Request.Path.StartsWithSegments(path)));
        }

        public AnalyticsModuleBuilder ExcludeExtension(params string[] extensions)
        {
            return Exclude(x => extensions.Any(ext => x.Request.Path.Value.EndsWith(ext)));
        }

        public AnalyticsModuleBuilder ExcludeMethods(params string[] methods)
        {
            return Exclude(x => methods.Any(ext => x.Request.Method == ext));
        }

        public AnalyticsModuleBuilder ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public AnalyticsModuleBuilder ExcludeIp(IPAddress address) => Exclude(x => x.Connection.RemoteIpAddress.Equals(address));

        public AnalyticsModuleBuilder ExcludeStatusCodes(params HttpStatusCode[] codes) => Exclude(context => codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleBuilder ExcludeStatusCodes(params int[] codes) => Exclude(context => codes.Contains(context.Response.StatusCode));

        public AnalyticsModuleBuilder LimitToStatusCodes(params HttpStatusCode[] codes) => Exclude(context => !codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleBuilder LimitToStatusCodes(params int[] codes) => Exclude(context => !codes.Contains(context.Response.StatusCode));
    }
}
