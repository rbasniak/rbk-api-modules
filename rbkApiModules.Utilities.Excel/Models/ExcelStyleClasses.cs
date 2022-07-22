using static rbkApiModules.Utilities.Excel.ClosedXMLDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a style sheet for the excel headers and column data
/// </summary>
public class ExcelStyleClasses
{
    /// <summary>
    /// Font that should be applied on the data set. Ex:"0" for Arial; "3" for Calibri, "8" for Georgia Pro, etc
    /// </summary>
    public ExcelFonts.FontName Font { get; set; } = ExcelFonts.FontName.Calibri;
    /// <summary>
    /// Font size following the excel app standard integer size
    /// </summary>
    public int FontSize { get; set; } = 11;
    /// <summary>
    /// If bold property should be applied
    /// </summary>
    public bool Bold { get; set; } = false;
    /// <summary>
    /// If Italic property should be applied
    /// </summary>
    public bool Italic { get; set; } = false;
}
