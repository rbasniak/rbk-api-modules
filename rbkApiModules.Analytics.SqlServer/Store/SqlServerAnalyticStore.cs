using Microsoft.EntityFrameworkCore;
using rbkApiModules.Analytics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.SqlServer
{ 
    /// <summary>
    /// Store para SQL Server
    /// </summary>
    public class SqlServerAnalyticStore : IAnalyticModuleStore
    {
        private readonly SqlServerAnalyticsContext _context;
        public SqlServerAnalyticStore(SqlServerAnalyticsContext context)
        {
            _context = context;
        }

        public void StoreData(AnalyticsEntry request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        }

        public async Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Data.Where(x => x.Timestamp >= from && x.Timestamp <= to).ToListAsync();
        }

        public async Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to, string[] versions, string[] areas, 
            string[] domains, string[] actions, string[] users, string[] agents, string[] responses, int duration, string entityId)
        {
            var query = _context.Data
                .Where(x => x.Timestamp >= from && x.Timestamp <= to)
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
                query = query.Where(x => responses.Any(response => x.Response.ToString() == response));
            }

            if (!String.IsNullOrEmpty(entityId))
            {
                query = query.Where(x => x.Path.Contains(entityId));
            }

            var result = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return result;
        }

        public async Task<List<AnalyticsEntry>> AllAsync()
        {
            return await _context.Data.ToListAsync();
        }
    }
}
