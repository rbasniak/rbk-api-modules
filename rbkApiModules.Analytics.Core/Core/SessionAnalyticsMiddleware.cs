using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class SessionAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        internal static List<SessionData> Sessions = new List<SessionData>();
        internal static bool IsLocked = false;
        internal static bool HasStarted = false;
        internal static int SessionTimeout;

        public SessionAnalyticsMiddleware(RequestDelegate next, AnalyticsModuleOptions options)
        {
            _next = next;
            SessionTimeout = options.SessionIdleLimit;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.User.Identity.IsAuthenticated)
                {
                    HasStarted = true;

                    if (!IsLocked)
                    {
                        var username = context.User.Identity.Name.ToLower();

                        var existingSession = Sessions.OrderByDescending(x => x.Start).FirstOrDefault(x => x.Username == username);

                        if (existingSession == null)
                        {
                            var session = new SessionData
                            {
                                Username = username,
                                Start = DateTime.UtcNow,
                                End = DateTime.UtcNow,
                            };

                            Sessions.Add(session);
                        }
                        else
                        {
                            if (existingSession.Inactivity <= SessionTimeout)
                            {
                                existingSession.End = DateTime.UtcNow;
                            }
                            else
                            {
                                var session = new SessionData
                                {
                                    Username = username,
                                    Start = DateTime.UtcNow,
                                    End = DateTime.UtcNow,
                                };

                                Sessions.Add(session);
                            }
                        }
                    }
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

