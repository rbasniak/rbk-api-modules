using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Analytics
{
    public class SqlServerWebRequest
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        [MaxLength(64)]
        public string Identity { get; set; }

        [MaxLength(64)]
        public string Username { get; set; }

        [MaxLength(32)]
        public string RemoteIpAddress { get; set; }

        public bool IsHttps { get; set; }

        [MaxLength(16)]
        public string Method { get; set; }

        [MaxLength(1024)]
        public string Path { get; set; }

        public int Response { get; set; }

        [MaxLength(512)]
        public string UserAgent { get; set; }

        public long Duration { get; set; } 
    }
}
