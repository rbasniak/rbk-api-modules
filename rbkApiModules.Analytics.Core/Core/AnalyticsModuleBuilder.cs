﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Http.Features;

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
            var bodyStream = context.Response.Body;
            context.Request.EnableBuffering();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                var originalBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
                var temp1 = context.Features.Get<IHttpBodyControlFeature>();
                //var temp2 = context.Features.Get<IHttpResponseControl>();

                //var compressionBody = new ResponseCompressionBody(context, _provider, originalBodyFeature);
                //context.Features.Set<IHttpResponseBodyFeature>(compressionBody);
                //context.Features.Set<IHttpsCompressionFeature>(compressionBody);

                var identity = UserIdentity(context);

                // TODO: get version from somewhere
                var data = new AnalyticsEntry();

                data.Version = "1.0.0";
                data.IpAddress = context.Connection.RemoteIpAddress.ToString();
                data.UserAgent = context.Request.Headers["User-Agent"];
                data.Path = context.Request.Method + " " + context.Request.Path;
                data.Method = context.Request.Method;

                //new MemoryStream. 
                using (var responseBody = new MemoryStream())
                {
                    if (!_exclude?.Any(x => x(context)) ?? true)
                    {
                        // temporary response body 
                        context.Response.Body = responseBody;
                    }

                    //execute the Middleware pipeline 
                    await _next(context);

                    if (!_exclude?.Any(x => x(context)) ?? true)
                    {
                        //read the response stream from the beginning
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        //Copy the contents of the new memory stream
                        await responseBody.CopyToAsync(bodyStream);

                        var responseSize = 0L;
                        if (context.Response.Body.CanSeek && context.Response.Body.CanRead)
                        {
                            responseSize = context.Response.Body.Length;
                        }

                        var requestSize = 0L;
                        if (context.Request.Body.CanSeek && context.Request.Body.CanRead)
                        {
                            requestSize = context.Request.Body.Length;
                        }

                        stopwatch.Stop();


                        var areaData = context.Items.FirstOrDefault(x => x.Key.ToString() == "log-data-area");
                        var pathData = context.Items.FirstOrDefault(x => x.Key.ToString() == "log-data-path");
                        var wasCached = context.Items.FirstOrDefault(x => x.Key.ToString() == "was-cached");

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

                        var transactionTime = -1;
                        var transactionCount = -1;

                        if (context.Items.TryGetValue("transaction-time", out object rawTime))
                        {
                            var time = (int)rawTime;
                            transactionTime = time;
                        }

                        if (context.Items.TryGetValue("transaction-count", out object rawCount))
                        {
                            var count = (int)rawCount;
                            transactionCount = count;
                        }

                        data.TransactionCount = transactionCount;
                        data.TotalTransactionTime = transactionTime;

                        //  Get identity of the user

                        var username = String.Empty;
                        var domain = String.Empty;

                        if (context.User.Identity.IsAuthenticated)
                        {
                            username = context.User.Identity.Name.ToLower();

                            var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
                            domain = user.Claims.FirstOrDefault(c => c.Type.ToLower() == "domain")?.Value;
                        }

                        data.Identity = identity;
                        data.Username = username;
                        data.Domain = domain;

                        var store = context.RequestServices.GetService<IAnalyticModuleStore>();

                        store.StoreData(data);
                    }
                }
            }
            finally
            {
                context.Response.Body = bodyStream;
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
        private bool _seedSampleDatabase;

        public AnalyticsModuleOptions Exclude(Func<HttpContext, bool> filter)
        {
            if (_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public List<Func<HttpContext, bool>> ExcludeRules => _exclude;

        public bool SeedSampleDatabase => _seedSampleDatabase;

        public AnalyticsModuleOptions UseDemoData()
        {
            _seedSampleDatabase = true;
            return this;
        }

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

