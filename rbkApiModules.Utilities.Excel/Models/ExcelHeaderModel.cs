
namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a full header row, data and styling.
/// </summary>
public class ExcelHeaderModel
{
    /// <summary>
    /// The data list to be displayed at the header row
    /// </summary>
    public string[] Data { get; set; }

    /// <summary>
    /// Sets the row height for the whole header row
    /// </summary>
    public int RowHeight { get; set; } = 0;

    /// <summary>
    /// Styles to be applied to the header row
    /// </summary>
    public ExcelStyleClasses Style { get; set; }

    #region Helper fields and methods

    /// <summary>
    /// Reserved Quick access Key Built from font, fontSize, data type and format
    /// </summary>
    public string StyleKey { get; private set; } = "";

    public void AddStyleKey(string key)
    {
        StyleKey = key;
    }

    #endregion
}