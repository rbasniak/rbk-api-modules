
namespace rbkApiModules.Utilities.Excel;

/// <summary>
/// Class representing a style sheet for the excel headers and column data
/// </summary>
public class ExcelStyleClasses
{
    /// <summary>
    /// Font that should be applied on the data set. Ex:"0" for Arial; "3" for Calibri, "8" for Georgia Pro, etc
    /// </summary>
    public ExcelModelDefs.ExcelFonts.FontType Font { get; set; } = ExcelModelDefs.ExcelFonts.FontType.Calibri;

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

    /// <summary>
    /// If a hex value is set, then the font will preferably use the Hex color value. If none is set, it will use the theme color
    /// Expects an 8 characters Hexadecimal ARGB string pattern without the # e.g. "FFFF0000" for solid Red.
    /// "FF00FF00" for solid green and "FF0000FF" for solid blue.
    /// </summary>
    public string FontColor { get; set; } = string.Empty;
}
