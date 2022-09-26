using static rbkApiModules.Utilities.Excel.ExcelModelDefs;

namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a style sheet for the excel headers and column data
/// </summary>
public class ExcelStyleClasses
{
    /// <summary>
    /// Font that should be applied on the data set. Ex:"0" for Arial; "3" for Calibri, "8" for Georgia Pro, etc
    /// </summary>
    public ExcelFonts.FontType Font { get; set; } = ExcelFonts.FontType.Calibri;

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

    /// <summary>
    /// If the font should display an underline
    /// </summary>
    public bool Underline { get; set; } = false;
}
