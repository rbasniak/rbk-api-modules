using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Auditing.Core
{
    public interface IAuditingModuleStore
    {
        void StoreData(StoredEvent request); 

        Task<List<StoredEvent>> InTimeRange(DateTime from, DateTime to); 
    }
}
