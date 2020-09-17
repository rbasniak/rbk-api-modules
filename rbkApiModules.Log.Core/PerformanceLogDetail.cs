using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Logging.Core
{
    public class PerformanceLogDetail
    {
        public DateTime ServerTimestamp { get; set; }
        public DateTime ClientTimestamp { get; set; }

        // WHERE
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public int Response { get; set; }
        public bool IsHttps { get; set; }

        public string UserAgent { get; set; }

        // WHO
        public string Identity { get; set; }
        public string Username { get; set; }
        public string RemoteIpAddress { get; set; }


        // EVERYTHING ELSE
        public long ElapsedMilliseconds { get; set; }  
        public string CorrelationId { get; set; } 
        public Dictionary<string, object> AdditionalInfo { get; set; } 
    }
}
