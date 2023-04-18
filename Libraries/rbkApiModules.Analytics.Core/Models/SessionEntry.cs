using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Localization;

public class SessionEntry
{
    public SessionEntry(string username, DateTime start, DateTime end)
    {
        Username = username;
        Start = start;
        End = end;
        Duration = (float)(end - start).TotalMinutes;
    }

    public Guid Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    [MaxLength(128)]
    public string Username { get; set; }
    public float Duration { get; set; }
}