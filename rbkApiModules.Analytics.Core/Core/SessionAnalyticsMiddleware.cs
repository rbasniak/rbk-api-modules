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
        private readonly AnalyticsModuleOptions _options;
        private static List<SessionData> Sessions = new List<SessionData>();

        public SessionAnalyticsMiddleware(RequestDelegate next, AnalyticsModuleOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

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

                        Sessions.Add(existingSession);
                    }
                    else
                    {
                        if (existingSession.Inactivity <= _options.SessionIdleLimit)
                        {
                            existingSession.End = DateTime.UtcNow;
                        }
                    }

                    var createNewUserSession = false;
                    var sessionsToRemove = new List<SessionData>();
                    foreach (var session in Sessions.Where(x => x.Inactivity >= _options.SessionIdleLimit))
                    {
                        var store = context.RequestServices.GetService<IAnalyticModuleStore>();
                        store.StoreSession(new SessionEntry(session.Username, session.Start, session.End));

                        if (session.Username == username)
                        {
                            createNewUserSession = true;
                        }
                    }

                    if (createNewUserSession)
                    {
                        existingSession = new SessionData
                        {
                            Username = username,
                            Start = DateTime.UtcNow,
                            End = DateTime.UtcNow,
                        };

                        Sessions.Add(existingSession);
                    }

                    Sessions = Sessions.Where(x => x.Inactivity < _options.SessionIdleLimit).ToList();
                }
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

