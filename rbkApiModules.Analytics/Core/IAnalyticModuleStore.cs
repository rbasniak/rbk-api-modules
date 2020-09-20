using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public interface IAnalyticModuleStore
    {
        void StoreData(AnalyticsEntry request); 
        
        Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to, string[] versions, string[] areas, string[] domains, 
            string[] actions, string[] users, string[] agents, string[] responses, int duration, string entityId);

        Task<List<AnalyticsEntry>> AllAsync();
    }
}
