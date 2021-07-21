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

        public async Task<List<AnalyticsEntry>> FilterAsync(DateTime from, DateTime to, string[] versions, string[] areas,
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

        public async Task<List<AnalyticsEntry>> FilterAsync(DateTime from, DateTime to)
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

        public async Task<List<AnalyticsEntry>> AllAsync()
        {
            return FixTimezone(await _context.Data.ToListAsync());
        }

        private List<AnalyticsEntry> FixTimezone(List<AnalyticsEntry> data)
        {
            return data.Select(x => x.FixTimezone(_timezoneOffsetours)).ToList();
        }

        public async Task<FilterOptionListData> AllFilteringLists()
        {
            var data = new FilterOptionListData();

            var analytics = await _context.Data.Select(x => new 
            {
                Date = x.Timestamp,
                Action = x.Action,
                UserAgent = x.UserAgent,
                Area = x.Area,
                Domain = x.Domain,
                Response = x.Response,
                Username = x.Username,
                Version = x.Version
            }).ToListAsync();

            data.StartDate = analytics.Last().Date;
            data.EndDate = analytics.First().Date;
            data.Actions = analytics.Select(x => x.Action).Distinct().OrderBy(x => x).ToList();
            data.Agents = analytics.Select(x => x.UserAgent).Distinct().OrderBy(x => x).ToList();
            data.Areas = analytics.Select(x => x.Area).Distinct().OrderBy(x => x).ToList();
            data.Domains = analytics.Select(x => x.Domain).Distinct().OrderBy(x => x).ToList();
            data.Responses = analytics.Select(x => x.Response.ToString()).Distinct().OrderBy(x => x).ToList();
            data.Users = analytics.Select(x => x.Username).Distinct().OrderBy(x => x).ToList();
            data.Versions = analytics.Select(x => x.Version).Distinct().OrderBy(x => x).ToList();

            return data;
        }

        public void StoreSession(SessionEntry session)
        {
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }
    }
}
