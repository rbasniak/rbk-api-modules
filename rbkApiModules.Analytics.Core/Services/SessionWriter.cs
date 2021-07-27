using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Utilities;

namespace rbkApiModules.Analytics.Core
{
    public class SessionWriter : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _services;
        private IHttpContextAccessor _httpContextAccessor;

        public SessionWriter(IHttpContextAccessor httpContextAccessor, IServiceProvider services)
        {
            _httpContextAccessor = httpContextAccessor;
            _services = services;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (!SessionAnalyticsMiddleware.HasStarted) return;

            SessionAnalyticsMiddleware.IsLocked = true;

            try
            {
                using (var scope = _services.CreateScope())
                {
                    foreach (var session in SessionAnalyticsMiddleware.Sessions.Where(x => x.Inactivity >= SessionAnalyticsMiddleware.SessionTimeout))
                    {
                        var store = scope.ServiceProvider.GetRequiredService<IAnalyticModuleStore>();
                        store.StoreSession(new SessionEntry(session.Username, session.Start, session.End));
                    }

                    SessionAnalyticsMiddleware.Sessions = SessionAnalyticsMiddleware.Sessions.Where(x => x.Inactivity < SessionAnalyticsMiddleware.SessionTimeout).ToList();
                }
            }
            catch (Exception ex)
            {
                using (var scope = _services.CreateScope())
                {
                    var diagnosticsStore = scope.ServiceProvider.GetRequiredService<IDiagnosticsModuleStore>();
                    diagnosticsStore.StoreData(new DiagnosticsEntry
                    {
                        ApplicationArea = "Analytics",
                        ApplicationLayer = "API",
                        ApplicationVersion = "",
                        ClientBrowser = "",
                        ClientDevice = "",
                        ClientOperatingSystem = "",
                        ClientOperatingSystemVersion = "",
                        ClientUserAgent = "",
                        DatabaseExceptions = "",
                        Domain = "",
                        ExceptionMessage = ex.Message,
                        ExceptionSource = "SessionsBackgroundService",
                        ExtraData = "",
                        InputData = "",
                        RequestId = "",
                        StackTrace = ex.ToBetterString(),
                        Timestamp = DateTime.UtcNow,
                        Username = "SYSTEM",
                    });
                }
            }

            SessionAnalyticsMiddleware.IsLocked = false;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
