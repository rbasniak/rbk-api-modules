namespace rbkApiModules.Commons.Localization;

public class PerformanceEntry
{
    public PerformanceEntry()
    {
    }

    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; }
    public long ResponseSize { get; set; }
    public long RequestSize { get; set; }
    public int Duration { get; set; }
    public bool HasError { get; set; }
    public string Username { get; set; }
    public int TotalTransactionTime { get; set; }
    public int TransactionCount { get; set; }

    public PerformanceEntry FixTimezone(double timezoneOffsetours)
    {
        Timestamp.AddHours(timezoneOffsetours);

        return this;
    }
}