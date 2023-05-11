namespace rbkApiModules.Diagnostics.Core;
public class FilterOptionListData
{
    public FilterOptionListData()
    {
        Levels = Array.Empty<string>();
        MessageTemplates = Array.Empty<string>();
    }

    public DateTime MinimumDate { get; set; }
    public string[] Levels { get; set; }
    public string[] MessageTemplates { get; set; }
} 