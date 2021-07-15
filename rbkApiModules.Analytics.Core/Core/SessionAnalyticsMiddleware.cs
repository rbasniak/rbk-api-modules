using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; 
using System.IO; 

namespace rbkApiModules.Analytics.Core
{
    public class SessionAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _useSessionsMiddleware;
        private readonly int _sessionInactivityLimit;
        private static List<SessionData> Sessions = new List<SessionData>();

        public SessionAnalyticsMiddleware(RequestDelegate next, AnalyticsModuleOptions options)
        {
            _next = next;
            //_useSessionsMiddleware = options.SessionMiddlewareEnabled;
            //_sessionInactivityLimit = options.SessionInactivityLimit;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    var username = context.User.Identity.Name.ToLower();

                    var existingSession = Sessions.SingleOrDefault(x => x.Username == username);

                    if (existingSession == null)
                    {
                        existingSession = new SessionData
                        {
                            Username = username,
                            Start = DateTime.UtcNow,
                            End = DateTime.UtcNow,
                        };
                    }
                    else
                    {
                        existingSession.End = DateTime.UtcNow;
                    }

                    var sessionsToRemove = new List<SessionData>();

                    foreach (var session in Sessions.Where(x => x.Inactivity >= _sessionInactivityLimit))
                    {
                        // var store = context.RequestServices.GetService<IAnalyticModuleStore>();
                        // store.StoreData(data);
                    }

                    Sessions = Sessions.Where(x => x.Inactivity < _sessionInactivityLimit).ToList();
                }

                await _next(context);
            }
            finally
            {
            }
        } 

        internal class SessionData
        {
            public string Username { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public double Duration => (End - Start).TotalMinutes;
            public double Inactivity => (DateTime.UtcNow - End).TotalMinutes;
        }
    }
}

