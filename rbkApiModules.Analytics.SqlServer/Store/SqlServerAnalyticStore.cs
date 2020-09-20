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

        public async Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to, List<string> areas, List<string> domains, List<string> actions, List<string> users, List<string> agents, List<string> methods, List<string> responses, int duration, string entityId)
        {
            return new List<AnalyticsEntry>();
        }

        public async Task<List<AnalyticsEntry>> AllAsync()
        {
            return new List<AnalyticsEntry>();
        }
    }
}
