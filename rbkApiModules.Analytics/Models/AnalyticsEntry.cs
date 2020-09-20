﻿using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsEntry
    {
        public AnalyticsEntry()
        {

        }

        public AnalyticsEntry(string version, string area, string identity, string username, string domain, string ip,   
            string userAgent, string path, string action, int response, int duration)
        {
            Version = version;
            Area = area;
            Timestamp = DateTime.UtcNow;
            Identity = identity;
            Username = username;
            Domain = domain;
            RemoteIpAddress = ip;
            UserAgent = userAgent;
            Path = path;
            Action = action;
            Response = response;
            Duration = duration;
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
        public string RemoteIpAddress { get; set; }

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
