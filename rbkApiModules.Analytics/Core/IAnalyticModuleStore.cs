using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public interface IAnalyticModuleStore
    {
        void StoreData(PerformanceData request); 

        Task<List<PerformanceData>> InTimeRange(DateTime from, DateTime to); 
    }
}
