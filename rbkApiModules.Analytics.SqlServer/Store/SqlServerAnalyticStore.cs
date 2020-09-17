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

        public void StoreData(PerformanceData request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        }

        public async Task<List<PerformanceData>> InTimeRange(DateTime from, DateTime to)
        {
            return await _context.Data.Where(x => x.Timestamp >= from && x.Timestamp <= to).ToListAsync();
        }
    }
}
