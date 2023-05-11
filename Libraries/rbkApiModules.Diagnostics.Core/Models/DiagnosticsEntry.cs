namespace rbkApiModules.Diagnostics.Core;

public class DiagnosticsEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string Properties { get; set; }
    public string Message { get; set; }
    public string Template { get; set; }
    public string Exception { get; set; }
}
