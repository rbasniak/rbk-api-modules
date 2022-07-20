using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a single spreadsheet inside an excel workbook.
/// </summary>
public class ExcelSheetModel
{
    /// <summary>
    /// Spreasheet tab name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// If the spreasheet should be sorted
    /// </summary>
    public bool ShouldSort { get; set; } = false;
    /// <summary>
    /// In case of sorting, if the sort should be case sensitive
    /// </summary>
    public bool MatchCase { get; set; } = false;
    /// <summary>
    /// In case of sorting, if the sorting should skip blanks, if false, will throw all blank entries at the top
    /// </summary>
    public bool IgnoreBlanks { get; set; } = true;
    /// <summary>
    /// In case of sorting, which column should the sorting be applied to
    /// </summary>
    public int SortColumn { get; set; } = 1;
    /// <summary>
    /// In case of sorting, if the sorting should be ascending or descending
    /// </summary>
    public ExcelSort.SortOrder SortOrder { get; set; } = ExcelSort.SortOrder.Ascending;
    /// <summary>
    /// The header data and styling container 
    /// </summary>
    public ExcelHeaderModel Header { get; set; }
    /// <summary>
    /// A list of all columns and their data/styling
    /// </summary>
    public ExcelColumnModel[] Columns { get; set; }
}
