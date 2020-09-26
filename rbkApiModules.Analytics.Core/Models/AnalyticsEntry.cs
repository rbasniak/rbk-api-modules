using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsEntry
    {
        public AnalyticsEntry()
        {
            Timestamp = DateTime.UtcNow;
        } 

        public Guid Id { get; set; }

        [MaxLength(32)]
        public string Version { get; set; }

        [MaxLength(32)]
        public string Area { get; set; }

        public DateTime Timestamp { get; set; }

        [MaxLength(128)]
        public string Identity { get; set; }

        [MaxLength(128)]
        public string Username { get; set; }

        [MaxLength(32)]
        public string Domain { get; set; }

        [MaxLength(64)]
        public string IpAddress { get; set; }

        [MaxLength(512)]
        public string UserAgent { get; set; }

        [MaxLength(16)]
        public string Method { get; set; }

        [MaxLength(256)]
        public string Path { get; set; }

        [MaxLength(256)]
        public string Action { get; set; }

        public long ResponseSize { get; set; }
        public long RequestSize { get; set; }
        public int Response { get; set; }

        public int Duration { get; set; }
        public bool WasCached { get; set; }

        public int TotalTransactionTime { get; set; }
        public int TransactionCount { get; set; }
    }
}
