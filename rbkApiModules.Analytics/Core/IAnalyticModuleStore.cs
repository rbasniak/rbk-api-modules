using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public interface IAnalyticModuleStore
    {
        void StoreData(AnalyticsEntry request); 
        Task<List<AnalyticsEntry>> InTimeRangeAsync(DateTime from, DateTime to, List<string> areas, List<string> domains, List<string> actions, List<string> users, List<string> agents, List<string> methods, List<string> responses, int duration, string entityId);
        Task<List<AnalyticsEntry>> AllAsync();
    }
}
