using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public interface IAnalyticModuleStore
    {
        void StoreData(AnalyticsEntry request); 
        
        Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to, string[] versions, string[] areas, string[] domains, 
            string[] actions, string[] users, string[] agents, string[] responses, string[] methods, int duration, string entityId);

        Task<List<AnalyticsEntry>> AllAsync();
    }
}
