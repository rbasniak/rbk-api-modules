using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using rbkApiModules.Analytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.SqlServer
{
    /// <summary>
    /// Store para SQL Server
    /// </summary>
    public class SqlServerAnalyticStore : IAnalyticModuleStore
    {
        private double _timezoneOffsetours = 0;
        private readonly SqlServerAnalyticsContext _context;

        public SqlServerAnalyticStore(SqlServerAnalyticsContext context, IOptions<RbkAnalyticsModuleOptions> options)
        {
            _context = context;

            _timezoneOffsetours = options.Value.TimezoneOffsetHours;
        }

        public void StoreData(AnalyticsEntry request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        } 

        public async Task<List<AnalyticsEntry>> FilterStatisticsAsync(DateTime from, DateTime to, string[] versions, string[] areas,
            string[] domains, string[] actions, string[] users, string[] agents, int[] responses, string[] methods, int duration, string entityId)
        {
            from = from.AddHours(_timezoneOffsetours);
            to = to.AddHours(_timezoneOffsetours);

            var query = _context.Data
                .Where(x => x.Timestamp.Date >= from.Date && x.Timestamp.Date <= to.Date)
                .Where(x => x.Duration >= duration);

            if (versions != null && versions.Length > 0)
            {
                query = query.Where(x => versions.Any(version => x.Version == version));
            }

            if (areas != null && areas.Length > 0)
            {
                query = query.Where(x => areas.Any(area => x.Area == area));
            }

            if (domains != null && domains.Length > 0)
            {
                query = query.Where(x => domains.Any(domain => x.Domain == domain));
            }

            if (actions != null && actions.Length > 0)
            {
                query = query.Where(x => actions.Any(action => x.Action == action));
            }

            if (users != null && users.Length > 0)
            {
                query = query.Where(x => users.Any(user => x.Username == user));
            }

            if (agents != null && agents.Length > 0)
            {
                query = query.Where(x => agents.Any(agent => x.UserAgent == agent));
            }

            if (responses != null && responses.Length > 0)
            {
                query = query.Where(x => responses.Any(response => x.Response == response));
            }

            if (methods != null && methods.Length > 0)
            {
                query = query.Where(x => methods.Any(method => x.Method == method));
            }

            if (!String.IsNullOrEmpty(entityId))
            {
                query = query.Where(x => x.Path.Contains(entityId));
            }

            var result = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return result;
        }

        public async Task<List<AnalyticsEntry>> FilterStatisticsAsync(DateTime from, DateTime to)
        {
            from = from.AddHours(_timezoneOffsetours);
            to = to.AddHours(_timezoneOffsetours);

            var query = _context.Data
                .Where(x => x.Timestamp.Date >= from.Date && x.Timestamp.Date <= to.Date);

            var sql = _context.Data
                .Where(x => x.Timestamp.Date >= from.Date && x.Timestamp.Date <= to.Date)
                .OrderByDescending(x => x.Timestamp).ToQueryString();

            var result = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return FixTimezone(result);
        }

        public async Task<List<AnalyticsEntry>> GetStatisticsAsync()
        {
            return FixTimezone(await _context.Data.ToListAsync());
        }

        private List<AnalyticsEntry> FixTimezone(List<AnalyticsEntry> data)
        {
            return data.Select(x => x.FixTimezone(_timezoneOffsetours)).ToList();
        }

        private List<PerformanceEntry> FixTimezone(List<PerformanceEntry> data)
        {
            return data.Select(x => x.FixTimezone(_timezoneOffsetours)).ToList();
        }

        public async Task<FilterOptionListData> GetFilteringLists()
        {
            var data = new FilterOptionListData();

            data.Actions = await _context.Data.Select(x => x.Action).Distinct().OrderBy(x => x).ToListAsync();
            data.Agents = await _context.Data.Select(x => x.UserAgent).Distinct().OrderBy(x => x).ToListAsync();
            data.Areas = await _context.Data.Select(x => x.Area).Distinct().OrderBy(x => x).ToListAsync();
            data.Domains = await _context.Data.Select(x => x.Domain).Distinct().OrderBy(x => x).ToListAsync();
            data.Responses = await _context.Data.Select(x => x.Response.ToString()).OrderBy(x => x).Distinct().ToListAsync();
            data.Users = await _context.Data.Select(x => x.Username).Distinct().OrderBy(x => x).ToListAsync();
            data.Versions = await _context.Data.Select(x => x.Version).Distinct().OrderBy(x => x).ToListAsync();

            data.StartDate = (await _context.Data.OrderBy(x => x.Timestamp).FirstAsync()).Timestamp.Date;
            data.EndDate = (await _context.Data.OrderBy(x => x.Timestamp).FirstAsync()).Timestamp.Date;

            return data;
        } 

        public void StoreSession(SessionEntry session)
        {
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }

        public async Task<List<SessionEntry>> GetSessionsAsync(DateTime dateFrom, DateTime dateTo)
        {
            return await _context.Sessions.Where(x => x.Start.Date >= dateFrom.Date && x.End.Date <= dateTo.Date).ToListAsync();
        }

        public void DeleteStatisticsFromMatchingPathAsync(string searchText)
        {
            _context.RemoveRange(_context.Data.Where(x => x.Path.ToLower().Contains(searchText.ToLower())));
        }

        public async Task<List<PerformanceEntry>> FilterPerformanceData(string endpoint, DateTime dateFrom, DateTime dateTo)
        {
            return await _context.Data
                .Where(x => x.Action == endpoint && x.Timestamp >= dateFrom && x.Timestamp <= dateTo && (x.Response == 200 || x.Response == 201 || x.Response == 204))
                .Select(x => new { x.Action, x.Duration, x.RequestSize, x.ResponseSize, x.Timestamp, x.TotalTransactionTime, x.TransactionCount })
                .Select(x => new PerformanceEntry { Action = x.Action, Duration = x.Duration, RequestSize = x.RequestSize, ResponseSize = x.ResponseSize, Timestamp = x.Timestamp, TransactionCount = x.TransactionCount, TotalTransactionTime = x.TotalTransactionTime })
                .ToListAsync();
        }

        public void NormalizePathsAndActions()
        {
            var results = _context.Data.ToList();

            foreach (var item in results)
            {
                if (item.Action.Contains(" api/"))
                {
                    item.Action = item.Action.Replace(" api/", " /api/");
                }

                if (item.Path.Contains(" api/"))
                {
                    item.Action = item.Path.Replace(" api/", " /api/");
                }
            }

            _context.SaveChanges();
        }
    }
}
