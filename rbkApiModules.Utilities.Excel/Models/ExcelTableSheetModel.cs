using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a single spreadsheet, holding table data, inside an excel workbook.
/// </summary>
public class ExcelTableSheetModel: ExcelBaseSheetModel
{
    /// <summary>
    /// The header data and styling container 
    /// </summary>
    public ExcelHeaderModel Header { get; set; }

    /// <summary>
    /// A list of all columns and their data/styling
    /// </summary>
    public ExcelColumnModel[] Columns { get; set; }

    /// <summary>
    /// If diferent from "None", applies a theme from excel's standard theme list to this spreadsheet
    /// </summary>
    public ExcelThemes.Theme Theme { get; set; } = ExcelThemes.Theme.None;


    #region Helper fields and methods

    /// <summary>
    /// Internal work variable that defines the start row if a column contains a subtotal row
    /// </summary>
    public int StartRow { get; private set; } = 1;

    internal void SetStartRow(int startRow)
    {
        StartRow = startRow;
    }

    #endregion
}
