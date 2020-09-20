using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsEntry
    {
        public Guid Id { get; set; }

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
        public string RemoteIpAddress { get; set; }

        [MaxLength(16)]
        public string Method { get; set; }

        [MaxLength(512)]
        public string UserAgent { get; set; }

        [MaxLength(256)]
        public string Path { get; set; }

        [MaxLength(256)]
        public string Action { get; set; }

        public int Response { get; set; }

        public int Duration { get; set; }
    }
}
