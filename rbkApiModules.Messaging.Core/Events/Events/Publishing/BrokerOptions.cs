namespace rbkApiModules.Commons.Core;

public class BrokerOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "events";
    public int PrefetchCount { get; set; } = 32;
    public TimeSpan Heartbeat { get; set; } = TimeSpan.FromSeconds(60);
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
}
