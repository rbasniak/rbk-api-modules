namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class for modeling an excel filter in case auto-filtering should be pre-define to a value
/// </summary>
public class FilterModel
{
    /// <summary>
    /// Filter type: Contains, Before, After, etc.
    /// </summary>
    public string FilterType { get; set; } = string.Empty;
    /// <summary>
    /// Query string for filtering
    /// </summary>
    public string Query { get; set; } = string.Empty;
}
