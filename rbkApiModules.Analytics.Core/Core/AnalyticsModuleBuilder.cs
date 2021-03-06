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
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsModuleMiddleware
    {
        private List<Func<HttpContext, bool>> _exclude;
        private readonly RequestDelegate _next;
        private string _version;
        public AnalyticsModuleMiddleware(RequestDelegate next, AnalyticsModuleOptions options)
        {
            _next = next;
            _exclude = options.ExcludeRules;
            _version = options.Version;
        }

        public async Task Invoke(HttpContext context)
        {
            var bodyStream = context.Response.Body;
            context.Request.EnableBuffering();

            try
            {
                var stopwatch = Stopwatch.StartNew();

                var identity = UserIdentity(context);

                // TODO: get version from somewhere
                var data = new AnalyticsEntry();

                data.Version = _version;
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


                        var areaData = context.Items.FirstOrDefault(x => x.Key.ToString() == AnalyticsMvcFilter.LOG_DATA_AREA);
                        var pathData = context.Items.FirstOrDefault(x => x.Key.ToString() == AnalyticsMvcFilter.LOG_DATA_PATH);
                        var wasCached = context.Items.FirstOrDefault(x => x.Key.ToString() == "was-cached");

                        if (areaData.Key != null)
                        {
                            data.Area = areaData.Value as string;
                        }

                        if (pathData.Key != null)
                        {
                            data.Action = context.Request.Method + " " + pathData.Value as string;
                        }

                        if (wasCached.Key != null)
                        {
                            data.WasCached = (bool)wasCached.Value;
                        }

                        data.Duration = (int)stopwatch.ElapsedMilliseconds;
                        data.Response = context.Response.StatusCode;
                        data.RequestSize = requestSize;
                        data.ResponseSize = responseSize;

                        var transactionTime = 0;
                        var transactionCount = 0;

                        if (context.Items.TryGetValue(DatabaseAnalyticsInterceptor.TRANSACTION_TIME_TOKEN, out object rawTime))
                        {
                            var time = (double)rawTime;
                            transactionTime = (int)time;
                        }

                        if (context.Items.TryGetValue(DatabaseAnalyticsInterceptor.TRANSACTION_COUNT_TOKEN, out object rawCount))
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

            string identity;

            if (!context.Request.Cookies.ContainsKey(cookieName))
            {
                identity = context.Request.Cookies.ContainsKey("ai_user")
                            ? context.Request.Cookies["ai_user"]
                            : context.Connection.Id;

                //if (!context.Response.HasStarted)
                //{
                //    context.Response.Cookies.Append("identity", identity.ToLower());
                //}
            }
            else
            {
                identity = context.Request.Cookies[cookieName].ToLower();
            }

            return identity.ToLower();
        }
    }

    public class AnalyticsModuleOptions
    {
        private List<Func<HttpContext, bool>> _exclude;
        private bool _seedSampleDatabase;
        private string _version;

        public AnalyticsModuleOptions()
        {
            _version = "-";
        }

        public AnalyticsModuleOptions Exclude(Func<HttpContext, bool> filter)
        {
            if (_exclude == null) _exclude = new List<Func<HttpContext, bool>>();
            _exclude.Add(filter);
            return this;
        }

        public List<Func<HttpContext, bool>> ExcludeRules => _exclude;

        public bool SeedSampleDatabase => _seedSampleDatabase;

        public string Version => _version;

        public AnalyticsModuleOptions UseDemoData()
        {
            _seedSampleDatabase = true;
            return this;
        }

        public AnalyticsModuleOptions SetApplicationVersion(string value)
        {
            _version = value;
            return this;
        }


        public AnalyticsModuleOptions LimitToPath(string path) => Exclude(x => !x.Request.Path.StartsWithSegments(path));

        public AnalyticsModuleOptions ExcludePath(params string[] paths)
        {
            return Exclude(x => paths.Any(path => x.Request.Path.StartsWithSegments(path)));
        }

        public AnalyticsModuleOptions ExcludeExtension(params string[] extensions)
        {
            return Exclude(x => extensions.Any(ext => x.Request.Path.Value.ToLower().EndsWith(ext.ToLower())));
        }

        public AnalyticsModuleOptions ExcludeMethods(params string[] methods)
        {
            return Exclude(x => methods.Any(method => x.Request.Method.ToLower() == method.ToLower()));
        }

        public AnalyticsModuleOptions ExcludeLoopBack() => Exclude(x => IPAddress.IsLoopback(x.Connection.RemoteIpAddress));

        public AnalyticsModuleOptions ExcludeIp(IPAddress address) => Exclude(x => x.Connection.RemoteIpAddress.Equals(address));

        public AnalyticsModuleOptions ExcludeStatusCodes(params HttpStatusCode[] codes) => Exclude(context => codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleOptions ExcludeStatusCodes(params int[] codes) => Exclude(context => codes.Contains(context.Response.StatusCode));

        public AnalyticsModuleOptions LimitToStatusCodes(params HttpStatusCode[] codes) => Exclude(context => !codes.Contains((HttpStatusCode)context.Response.StatusCode));

        public AnalyticsModuleOptions LimitToStatusCodes(params int[] codes) => Exclude(context => !codes.Contains(context.Response.StatusCode));
    } 

}

