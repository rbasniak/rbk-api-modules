namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class with the data models and styling for a column data, to be displayed under a header title.
/// </summary>
public class ExcelColumnModel
{
    /// <summary>
    /// List of all data to be displayed on one column
    /// </summary>
    public string[] Data { get; set; }
    /// <summary>
    /// Styles to be applied to this column's data
    /// </summary>
    public ExcelStyleClasses Style { get; set; }
}
