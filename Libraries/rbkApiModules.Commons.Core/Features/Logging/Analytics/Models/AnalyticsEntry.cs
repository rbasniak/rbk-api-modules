using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public class AnalyticsEntry
{
    private string _domain;
    private string _username;

    public Guid Id { get; set; }
    public string Version { get; set; }
    public DateTime Timestamp { get; set; }
    public string Identity{ get; set; }
    public string Username
    {
        get
        {
            return _username ?? String.Empty;
        }
        set
        {
            _username = value;
        }
    }
    public string Tenant
    {
        get
        {
            return _domain ?? String.Empty;
        }
        set
        {
            _domain = value;
        }
    }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string Action { get; set; }
    public int Response { get; set; }
    public int Duration { get; set; }
    public bool WasCached { get; set; }
    public int TotalTransactionTime { get; set; }
    public int TransactionCount { get; set; }

    public AnalyticsEntry FixTimezone(double timezoneOffsetours)
    {
        Timestamp.AddHours(timezoneOffsetours);

        return this;
    }
}
