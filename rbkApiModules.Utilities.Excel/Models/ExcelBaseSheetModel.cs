using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Base sheet definition with tab name and color and the type of data to be displayed. Ex: Table Data, Plots, etc.
/// </summary>
public class ExcelBaseSheetModel
{
    /// <summary>
    /// Spreasheet tab name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Sets the spreadsheet's tab background color. By default, it will not set a background color.
    /// Sets the text color using an Enum based on System.Drawing Color names. Ex: "Black", "Red", "Gray", etc.
    /// </summary>
    public ExcelColors.Color TabColor { get; set; } = ExcelColors.Color.NoColor;
    /// <summary>
    /// Data type to be exhibited on the sheet tab. Ex: Table Data, Plots, etc.
    /// </summary>
    public ExcelSheetTypes.Type SheetType { get; set; } = ExcelSheetTypes.Type.Table;
    /// <summary>
    /// Tab order in which this sheet should be created 
    /// </summary>
    public int TabIndex { get; set; }
}
